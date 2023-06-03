using System.Collections.Generic;
using System.Text;

/// <summary>
/// 树形菜单数据
/// </summary>
public class TreeViewData
{
	/// <summary>
	/// 数据内容
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 数据所属的父ID
	/// </summary>
	public int ParentID { get; set; }

	public object Tag { get; set; }

	public bool IsRoot => ParentID == -1;

	private TreeViewControl parentView;

	public List<int> ChildIDs { get; } = new List<int>();

	public TreeViewData(TreeViewControl parentView)
	{
		this.parentView = parentView;
	}

	public TreeViewData(TreeViewControl parentView, string name, int parentID, object tag)
	{
		this.parentView = parentView;
		Name = name;
		ParentID = parentID;
		Tag = tag;
	}

	public string GetFullPathString()
	{
		var sb = new StringBuilder();
		sb.Append(Name);
		var parent = ParentID;
		while (parent != -1 && parent < parentView.Data.Count)
		{
			sb.Insert(0, parentView.Data[parent].Name + "/");
			parent = parentView.Data[parent].ParentID;
		}
		return sb.ToString();
	}

	public override string ToString() => GetFullPathString();
}