using System;
using System.Collections.Generic;
using System.Net.Mime;
using JetBrains.Annotations;
using static UnityEditor.Progress;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;

public class Inventory
{
    // Выбор нового IInventoryItem. params: last index, new index
    public UnityEvent<int, int> OnCnangeIndex { get; } = new();

    // Подбор IInventoryItem. params: selected index
    public UnityEvent<int> OnPickUpItem { get; } = new();

    // Выброс IInventoryItem. params: selected index
    public UnityEvent<int> OnDropItem { get; } = new();

    public List<IInventoryItem> Items { get; }
    // public List<Image> Items { get; }

    public int Capacity { get; }

    [CanBeNull]
    public IInventoryItem SelectedItem
    {
        get => Items[SelectedIndex];
        set
        {
            Items[_selectedIndex] = value;
            if (value != null)
                OnPickUpItem.Invoke(_selectedIndex);
        } // might be better to check if no item already here
    }

    private int _selectedIndex = 0;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < 0 || value >= Capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            SelectedItem?.OnUnselect();
            OnCnangeIndex.Invoke(_selectedIndex, value);
            _selectedIndex = value;
            SelectedItem?.OnSelect();
        }
    }

    public Inventory(int capacity = 4)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
        Items = new(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Items.Add(null);
        }
    }

    public void SelectNext()
    {
        SelectedIndex = (SelectedIndex + 1) % Capacity;
    }

    public void SelectPrevious()
    {
        SelectedIndex = (Capacity + SelectedIndex - 1) % Capacity;
    }

    public bool TryDropCurrent()
    {
        if (SelectedItem == null)
        {
            return false;
        }
        SelectedItem.OnUnselect();
        SelectedItem.DropOut();
        SelectedItem = null;
        OnDropItem.Invoke(_selectedIndex);

        return true;
    }
}