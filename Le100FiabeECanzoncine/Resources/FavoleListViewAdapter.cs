using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using System.Threading.Tasks;
using Android.Graphics;
using System.Net.Http;

namespace FiabeSenzaTempo
{
	public class FavoleListViewAdapter :BaseAdapter<videoItem>
	{
		private List<videoItem> m_theVideos = new List<videoItem>();
		private Context m_context;
		private View ThisRow;
		private ImageView theImage;
		private ListView m_theListView;
		private TextView theTitle;

		public FavoleListViewAdapter (Context context, List<videoItem> theVideos, ListView theListView)
		{
			m_context = context;
			m_theVideos = theVideos;
			m_theListView = theListView;
		}
		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}
			
		public override Android.Views.View GetView (int position, View convertView, Android.Views.ViewGroup parent)
		{
			ThisRow = convertView;
			if (ThisRow == null) {
				ThisRow = LayoutInflater.From(m_context).Inflate(Resource.Layout.FavoleListeViewItem,null,false);
				theTitle = (TextView)ThisRow.FindViewById (Resource.Id.textViewTitle);
				theTitle.Click += (sender, args) => m_theListView.PerformItemClick(ThisRow, position, GetItemId(position));
			}
			//TextView theTitle = (TextView)ThisRow.FindViewById (Resource.Id.textViewTitle);
			theTitle = (TextView)ThisRow.FindViewById (Resource.Id.textViewTitle);
			theTitle.Text = m_theVideos [position].Title.ToUpper();
			theImage = ThisRow.FindViewById<ImageView>(Resource.Id.imageViewTitle);
			theImage.SetImageBitmap(m_theVideos [position].Image);

			return ThisRow;	
		}
			
		public override int Count {
			get {
				return m_theVideos.Count;
			}
		}

		public override videoItem this [int index] {
			get {
				return m_theVideos [index];
			}
		}

		#endregion
	}
}

