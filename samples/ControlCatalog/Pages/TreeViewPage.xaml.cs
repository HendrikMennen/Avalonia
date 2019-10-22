using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ControlCatalog.Pages
{
    public class TreeViewPage : UserControl
    {
        public TreeViewPage()
        {
            InitializeComponent();
            DataContext = new PageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private class PageViewModel : ReactiveObject
        {
            private SelectionMode _selectionMode;

            public PageViewModel()
            {
                Node root = new Node();
                Items = root.Children;
                SelectedItems = new ObservableCollection<Node>();

                AddItemCommand = ReactiveCommand.Create(() =>
                {
                    Node parentItem = SelectedItems.Count > 0 ? SelectedItems[0] : root;
                    parentItem.AddNewItem();
                });

                RemoveItemCommand = ReactiveCommand.Create(() =>
                {
                    while (SelectedItems.Count > 0)
                    {
                        Node lastItem = SelectedItems[0];
                        RecursiveRemove(Items, lastItem);
                        SelectedItems.Remove(lastItem);
                    }

                    bool RecursiveRemove(ObservableCollection<Node> items, Node selectedItem)
                    {
                        if (items.Remove(selectedItem))
                        {
                            return true;
                        }

                        foreach (Node item in items)
                        {
                            if (item.AreChildrenInitialized && RecursiveRemove(item.Children, selectedItem))
                            {
                                return true;
                            }
                        }

                        return false;
                    }
                });
            }

            public ObservableCollection<Node> Items { get; }

            public ObservableCollection<Node> SelectedItems { get; }

            public ReactiveCommand<Unit, Unit> AddItemCommand { get; }

            public ReactiveCommand<Unit, Unit> RemoveItemCommand { get; }

            public SelectionMode SelectionMode
            {
                get => _selectionMode;
                set
                {
                    SelectedItems.Clear();
                    this.RaiseAndSetIfChanged(ref _selectionMode, value);
                }
            }

            public void SortParent()
            {               
                var tmp = Items[0];
                Items.RemoveAt(0);
                Items.Insert(1, tmp);
            }

            public void SortChild()
            {
                var node = Items[0];
                var tmp = node.Children[0];
                node.Children.RemoveAt(0);
                node.Children.Insert(1, tmp);
            }
        }

        private class Node
        {
            public bool IsExpanded { get; set; }

            private int _counter;
            private ObservableCollection<Node> _children;

            public string Header { get; private set; }

            public bool AreChildrenInitialized => _children != null;

            public ObservableCollection<Node> Children
            {
                get
                {
                    if (_children == null)
                    {
                        _children = new ObservableCollection<Node>(Enumerable.Range(1, 2).Select(i => CreateNewNode()));
                    }
                    return _children;
                }
            }

            public void AddNewItem() => Children.Add(CreateNewNode());

            public override string ToString() => Header;

            private Node CreateNewNode() => new Node {Header = $"Item {_counter++}"};
        }
    }
}
