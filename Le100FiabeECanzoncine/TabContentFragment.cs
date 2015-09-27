
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FiabeSenzaTempo
{
	public class TabContentFragment : Fragment
	{
		private int tabIndex = 0;
		private ListView m_myList;
		private FavoleListViewAdapter m_adapter;
		View m_theView;
		List<videoItem> m_theVideos = new List<videoItem>();

		public TabContentFragment(int index)
		{
			tabIndex = index;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);		
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.TabContent, container, false);
			m_theView = view;

			foreach (var current in MainActivity.m_theVideos) {
				if (current.Page == tabIndex.ToString())
					m_theVideos.Add (current);
			}

			// set the list and adapter
			m_myList = (ListView)m_theView.FindViewById (Resource.Id.myListView);
			m_adapter = new FavoleListViewAdapter (m_theView.Context, m_theVideos );
			m_myList.Adapter = m_adapter;
			m_myList.ItemClick += M_myList_ItemClick;


			return view;
		}
		void M_myList_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			System.Console.WriteLine ("M_myList_ItemClick " + e.Position.ToString ());
			videoItem sellectedItem = MainActivity.m_theVideos [e.Position];
			string videoID = sellectedItem.URL.Split (new char[] { '=' })[1];
			Intent intent = new Intent(((View)m_theView).Context, typeof(TheViewViewer));
			intent.PutExtra ("videoID", videoID);
			this.StartActivity (intent);

		}

		public void SetTabIndex(int index)
		{
			tabIndex = index;
			/*
			int counter = 0;
			foreach (var current in MainActivity.m_theVideos) {
				if (counter % 3 == tabIndex)
					m_theVideos.Add (current);
				counter++;
			}
*/


		}
	}
}

