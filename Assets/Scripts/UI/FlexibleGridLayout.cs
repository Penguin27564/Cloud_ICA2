using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    [SerializeField]
    private int _rows, _columns;

    [SerializeField]
    private Vector2 _cellSize, _spacing;

    [SerializeField]
    private FitType _fitType;

    [SerializeField]
    private bool _fitX, _fitY, _fixedSize;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (_fitType == FitType.Width || _fitType == FitType.Height || _fitType == FitType.Uniform)
        {
            _fitX = true;
            _fitY = true;
            float sqrRt = Mathf.Sqrt(transform.childCount);
            _rows = Mathf.CeilToInt(sqrRt);
            _columns = Mathf.CeilToInt(sqrRt);
        }

        if (_fitType == FitType.Width || _fitType == FitType.FixedColumns)
        {
            _rows = Mathf.CeilToInt(transform.childCount / (float)_columns);
        }
        else if (_fitType == FitType.Height || _fitType == FitType.FixedRows)
        {
            _columns = Mathf.CeilToInt(transform.childCount / (float)_rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)_columns) - ((_spacing.x / (float)_columns) * (_columns - 1))
                        - (padding.left / (float)_columns) - (padding.right / (float)_columns);

        float cellHeight = (parentHeight / (float)_rows) - ((_spacing.y / (float)_rows) * (_rows - 1))
                        - (padding.top / (float)_rows) - (padding.bottom / (float)_rows);

        _cellSize.x = _fitX ? cellWidth : _cellSize.x;
        _cellSize.y = _fitY ? cellHeight : _cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / _columns;
            columnCount = i % _columns;

            var item = rectChildren[i];

            var xPos = (_cellSize.x * columnCount) + (_spacing.x * columnCount) + padding.left;
            var yPos = (_cellSize.y * rowCount) + (_spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, _cellSize.x);
            SetChildAlongAxis(item, 1, yPos, _cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
