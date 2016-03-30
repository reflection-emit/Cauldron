using System.Windows.Controls;

namespace Couldron.Behaviours
{
    public sealed class TreeViewCollapseAllBehaviour : TreeViewExpandCollapseToggleBehaviourBase
    {
        protected override bool IsExpanded
        {
            get
            {
                return false;
            }
        }
    }

    public sealed class TreeViewExpandAllBehaviour : TreeViewExpandCollapseToggleBehaviourBase
    {
        protected override bool IsExpanded
        {
            get
            {
                return true;
            }
        }
    }

    public abstract class TreeViewExpandCollapseToggleBehaviourBase : BehaviourInvokeAwareBehaviourBase<TreeView>
    {
        protected abstract bool IsExpanded { get; }

        protected override void Invoke()
        {
            this.SetIsExpand(this.AssociatedObject as ItemsControl, this.IsExpanded);
        }

        protected override void OnAttach()
        {
        }

        protected override void OnDetach()
        {
        }

        private void SetIsExpand(ItemsControl itemsControl, bool value)
        {
            if (itemsControl == null)
                return;

            foreach (var item in itemsControl.Items)
            {
                var childControl = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as ItemsControl;
                if (childControl != null)
                    this.SetIsExpand(childControl, value);

                (childControl as TreeViewItem).IsNotNull(x => x.IsExpanded = value);
            }
        }
    }
}