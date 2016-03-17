using System;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace FineHouse
{
	public class ToDoItemAdapter : BaseAdapter<CreateUserBindingModel>
	{
		Activity activity;
		int layoutResourceId;
		List<CreateUserBindingModel> items = new List<CreateUserBindingModel> ();

		public ToDoItemAdapter (Activity activity, int layoutResourceId)
		{
			this.activity = activity;
			this.layoutResourceId = layoutResourceId;
		}

		//Returns the view for a specific item on the list
		public override View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var row = convertView;
			var currentItem = this [position];
			CheckBox checkBox;

			if (row == null) {
				var inflater = activity.LayoutInflater;
				row = inflater.Inflate (layoutResourceId, parent, false);

				checkBox = row.FindViewById <CheckBox> (Resource.Id.checkToDoItem);

				
					}
				
			
				

			return row;
		}

		public void Add (CreateUserBindingModel item)
		{
			items.Add (item);
			NotifyDataSetChanged ();
		}

		public void Clear ()
		{
			items.Clear ();
			NotifyDataSetChanged ();
		}

		public void Remove (CreateUserBindingModel item)
		{
			items.Remove (item);
			NotifyDataSetChanged ();
		}

		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count {
			get {
				return items.Count;
			}
		}

		public override CreateUserBindingModel this [int position] {
			get {
				return items [position];
			}
		}

		#endregion
	}
}

