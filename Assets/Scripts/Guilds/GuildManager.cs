using PlayFab;
using PlayFab.GroupsModels;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using EntityKey = PlayFab.GroupsModels.EntityKey;
using EmptyResponse = PlayFab.GroupsModels.EmptyResponse;

[Serializable]
public class GuildManager : MonoBehaviour
{
    private static GuildManager _instance;

    public static GuildManager Instance
    {
        get
        {
            _instance ??= new GuildManager();
            return _instance;
        }
    }

    public GuildManager()
    {
        _instance = this;
    }
    
    // A local cache of some bits of PlayFab data
    // This cache pretty much only serves this example , and assumes that entities are uniquely identifiable by EntityId alone, which isn't technically true. Your data cache will have to be better.
    public readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public readonly Dictionary<string, string> GroupNameById = new Dictionary<string, string>();
    public EntityKey currentGroupKey;
    public List<UserElement> currentGuildMembers = new();

    public Action OnMemberRemoved, OnInviteInteraction;

    public static EntityKey EntityKeyMaker(string entityId)
    {
        return new EntityKey { Id = entityId };
    }

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void ListGroups(EntityKey entityKey)
    {
        var request = new ListMembershipRequest { Entity = entityKey };
        PlayFabGroupsAPI.ListMembership(request, OnListGroups, OnSharedError);
    }

    private void OnListGroups(ListMembershipResponse response)
    {
        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
        }
    }

    public void CreateGroup(string groupName, List<EntityKey> initialMembers)
    {
        // A player-controlled entity creates a new group
        var request = new CreateGroupRequest { GroupName = groupName };
        PlayFabGroupsAPI.CreateGroup(request,
        result =>
        {
            Debug.Log("Group Created: " + result.GroupName + " - " + result.Group.Id);
            MessageBoxManager.Instance.DisplayMessage("Guild successfully created!");

            if (initialMembers.Count > 0)
            {
                var csrequest = new ExecuteCloudScriptRequest
                {
                    FunctionName = "AddMembersToGroup",
                    FunctionParameter = new
                    {
                        GroupId = result.Group.Id,
                        MemberIDs = initialMembers
                    }
                };

                PlayFabClientAPI.ExecuteCloudScript(csrequest,
                result =>
                {
                    Debug.Log("Added members!");
                }, OnSharedError);
            }

        }, OnSharedError);
    }
    public void DeleteGroup(string groupId)
    {
        // A title, or player-controlled entity with authority to do so, decides to destroy an existing group
        var request = new DeleteGroupRequest { Group = EntityKeyMaker(groupId) };
        PlayFabGroupsAPI.DeleteGroup(request, OnDeleteGroup, OnSharedError);
    }
    private void OnDeleteGroup(EmptyResponse response)
    {
        var prevRequest = (DeleteGroupRequest)response.Request;
        Debug.Log("Group Deleted: " + prevRequest.Group.Id);

        var temp = new HashSet<KeyValuePair<string, string>>();
        foreach (var each in EntityGroupPairs)
            if (each.Value != prevRequest.Group.Id)
                temp.Add(each);
        EntityGroupPairs.IntersectWith(temp);
        GroupNameById.Remove(prevRequest.Group.Id);
    }
    public void InviteMember(EntityKey userKey)
    {
        var request = new InviteToGroupRequest { Group = currentGroupKey, Entity = userKey};
        PlayFabGroupsAPI.InviteToGroup(request,
        result =>
        {
        }, OnSharedError);
    }
    public void AcceptInvite(EntityKey groupKey)
    {
        var request = new AcceptGroupInvitationRequest { Group = groupKey };
        PlayFabGroupsAPI.AcceptGroupInvitation(request,
        result =>
        {  
            MessageBoxManager.Instance.DisplayMessage("Invite accepted!");
            OnInviteInteraction.Invoke();
        }, OnSharedError);
    }
    public void DeclineInvite(EntityKey groupKey, EntityKey userKey)
    {
        var request = new RemoveGroupInvitationRequest { Group = groupKey, Entity = userKey };
        PlayFabGroupsAPI.RemoveGroupInvitation(request,
        result =>
        {  
            MessageBoxManager.Instance.DisplayMessage("Invite rejected :(");
            OnInviteInteraction.Invoke();
        }, OnSharedError);
    }
    public void ApplyToGroup(string groupId)
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(groupId) };
        PlayFabGroupsAPI.ApplyToGroup(request, OnApply, OnSharedError);
    }
    public void OnApply(ApplyToGroupResponse response)
    {
    }
    public void AcceptApplication(EntityKey applicantKey)
    {
        var request = new AcceptGroupApplicationRequest { Group = currentGroupKey, Entity = applicantKey };
        PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnSharedError);
    }
    public void OnAcceptApplication(EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
        
        MessageBoxManager.Instance.DisplayMessage("Accepted application!");
    }
    public void RejectApplication(EntityKey applicantKey)
    {
        var request = new RemoveGroupApplicationRequest { Group = currentGroupKey, Entity = applicantKey};
        PlayFabGroupsAPI.RemoveGroupApplication(request,
        result =>
        {
            MessageBoxManager.Instance.DisplayMessage("Rejected application");
        },
        error =>
        {
            MessageBoxManager.Instance.DisplayMessage("Error rejecting application");
        });
    }
    public void KickMember(EntityKey entityKey)
    {
        var request = new RemoveMembersRequest { Group = currentGroupKey, Members = new List<EntityKey> { entityKey } };
        PlayFabGroupsAPI.RemoveMembers(request, 
        result =>
        {
            Debug.Log("Entity kicked from Group: " + request.Members[0].Id + " to " + request.Group.Id);
            MessageBoxManager.Instance.DisplayMessage("Removed member :(");
            EntityGroupPairs.Remove(new KeyValuePair<string, string>(request.Members[0].Id, request.Group.Id));
            OnMemberRemoved.Invoke();
        }, 
        error =>
        {
            MessageBoxManager.Instance.DisplayMessage("Error removing member");
            Debug.LogError(error.GenerateErrorReport());
        });
    }
    public void LeaveGroup()
    {
        var request = new RemoveMembersRequest
        { 
            Group = currentGroupKey,
            Members = new List<EntityKey>{PFDataMgr.Instance.currentPlayEntityKey} 
        };

        PlayFabGroupsAPI.RemoveMembers(request, 
        result =>
        {
            Debug.Log("Left guild");
            MessageBoxManager.Instance.DisplayMessage("Left guild");
            EntityGroupPairs.Remove(new KeyValuePair<string, string>(request.Members[0].Id, request.Group.Id));
            OnMemberRemoved.Invoke();
        }, 
        error =>
        {
            MessageBoxManager.Instance.DisplayMessage("Error removing member");
            Debug.LogError(error.GenerateErrorReport());
        });
    }

}