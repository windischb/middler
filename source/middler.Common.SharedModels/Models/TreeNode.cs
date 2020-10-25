using System;
using System.Collections.Generic;
using System.Linq;

//namespace middler.Common.SharedModels.Models
//{
//    public class TreeNode
//    {
//        public Guid Id { get; set; }

//        public string Parent { get; set; }
//        public string Name { get; set; }

//        public string Path => $"{Parent}/{Name}".Trim('/');

//        public bool IsFolder { get; set; }

//        public string Extension { get; set; }

//        public object Content { get; set; }

//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public List<TreeNode> Children { get; set; }
//    }

//    public static class TreeNodeExtensions
//    {
//        public static TreeNode GetNodeByPath(this TreeNode treeNode, string path)
//        {
//            var splitted = path.Split('/');
//            var current = treeNode;
//            foreach (var s in splitted)
//            {
//                current = current.Children.FirstOrDefault(n => n.Name == s);
//                if (current == null)
//                {
//                    return null;
//                }
//            }

//            return current;
//        }
        
//    }
//}
