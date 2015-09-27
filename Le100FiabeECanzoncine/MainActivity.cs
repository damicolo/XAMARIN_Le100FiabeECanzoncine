﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Net;
using FiabeSenzaTempo;
using Android.Gms.Ads;
using System.Threading.Tasks;
using Android.Graphics;
using System.Net.Http;
using Java.IO;



namespace FiabeSenzaTempo
{
	[Activity (Label = "Le 100 Fiabe e Canzoncine", MainLauncher = true, Icon = "@drawable/AppIcon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private const string m_playlist = "http://damicolo1.azurewebsites.net/100FiabeECanzoncine/100FiabeECanzoncine.txt";
		//private ListView m_myList;
		//private FavoleListViewAdapter m_adapter;
		public static List<videoItem> m_theVideos = new List<videoItem>();

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;



			// prevent screen lock; require WAKE_LOCK permission in the manifest
			this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

			// retrieve the playlist from azure
			WebClient httpclient = new WebClient (); 
			string theFileList = "";
			try{
				theFileList = httpclient.DownloadString (m_playlist);
				theFileList = theFileList.Replace ("\r\n", "^");
			}
			catch(Exception ex) {
				System.Console.WriteLine (ex.ToString ());
			}
			string[] VideoList = theFileList.Split (new char[] { '^'});
			List<string> myData = new List<string> ();
			foreach (string s in VideoList) {
				if (s.StartsWith("#")) continue;
				string[] elements = s.Split (new char[]{ ';' });
				if (elements.Length < 4)
					continue;

				videoItem tempItem = new videoItem (){Page = elements[0], Title = elements [1], URL = elements [2], ImageURL = elements [3] };
				//tempItem.Image = await GetImageFromUrl(tempItem.ImageURL);
				m_theVideos.Add (tempItem);
				myData.Add (m_theVideos[m_theVideos.Count-1].Title);
			}


			// parellel download of all the miniatures
			var downloadTasksQuery = new Task<Bitmap>[m_theVideos.Count];
			for (int i = 0; i < m_theVideos.Count; i++) {
				downloadTasksQuery [i] = GetImageFromUrl (m_theVideos [i].ImageURL);
			}				
			Bitmap[] myImages = await Task.WhenAll (downloadTasksQuery);
			for (int i = 0; i < m_theVideos.Count; i++) {
				m_theVideos [i].Image = myImages [i];
			}
				


			AddTab ("Fiabe", 1, Resource.Drawable.favolesenzatempo,  new TabContentFragment(1) );
			AddTab ("Zecchino", 2, Resource.Drawable.favolesenzatempo,  new TabContentFragment(2) );
			AddTab ("Canzoncine", 3, Resource.Drawable.favolesenzatempo,  new TabContentFragment(3) );
			AddTab ("NinnaNanna", 4, Resource.Drawable.favolesenzatempo,  new TabContentFragment(4) );






			// advertising setup
			AdView mAdView = (AdView) this.FindViewById(Resource.Id.adView);
			AdRequest adRequest = new AdRequest.Builder ().Build ();
			mAdView.LoadAd(adRequest);
		}

		void AddTab (string tabText,int tabindex, int iconResourceId, Fragment fragment)
		{
			var tab = this.ActionBar.NewTab ();            
			tab.SetText (tabText);
			//tab.SetIcon (iconResourceId);
			((TabContentFragment)fragment).SetTabIndex (tabindex);
			// must set event handler for replacing tabs tab
			tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
				e.FragmentTransaction.Replace(Resource.Id.fragmentContainer, fragment);
			};

			this.ActionBar.AddTab (tab);
		}      


		private async Task<Bitmap> GetImageFromUrl(string url)
		{
			// let's check if the file exists...
			ContextWrapper cw = new ContextWrapper (this.ApplicationContext);
			File directory = cw.GetDir("imgDir", FileCreationMode.Private);
			string[] elements = url.Split (new char[] { '/' });

			bool fileExists = false;
			string fileName = directory + "/" + elements [elements.Length - 1];
			if (System.IO.File.Exists (fileName)) {
				fileExists = true;
				var imageFile = new Java.IO.File(fileName);
				Bitmap bitmap = BitmapFactory.DecodeFile(imageFile.AbsolutePath);
				return bitmap;
			}



			if (!fileExists) {
				using (var client = new HttpClient ()) {
					var msg = await client.GetAsync (url);
					if (msg.IsSuccessStatusCode) {
						using (var stream = await msg.Content.ReadAsStreamAsync ()) {						﻿
							var bitmap = await BitmapFactory.DecodeStreamAsync (stream);
							File myPath = new File (directory, elements [elements.Length - 1]);
							try {
								using (var os = new System.IO.FileStream (myPath.AbsolutePath, System.IO.FileMode.Create)) {
									bitmap.Compress (Bitmap.CompressFormat.Png, 100, os);
								}
							} catch (Exception ex) {
								System.Console.Write (ex.Message);
							}				
							return bitmap;
						}
					}
				}
			}
			return null;
		}

		void M_myList_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			System.Console.WriteLine ("M_myList_ItemClick " + e.Position.ToString ());
			videoItem sellectedItem = m_theVideos [e.Position];
			string videoID = sellectedItem.URL.Split (new char[] { '=' })[1];

			Intent intent = new Intent(this, typeof(TheViewViewer));
			intent.PutExtra ("videoID", videoID);
			this.StartActivity (intent);


			/*
			((LinearLayout.LayoutParams)videoView.LayoutParameters).Weight = 40f;
			try
			{
				YouTubeUri theURI = await  YouTube.GetVideoUriAsync(videoID,YouTubeQuality.Quality720P);
				var uri = Android.Net.Uri.Parse(theURI.Uri.AbsoluteUri);
				videoView.SetVideoURI(uri);
				videoView.Start ();

				m_videoPregressTimer.Enabled = true;
				m_videoPregressTimer.Start();
			}
			catch (Exception ex) 
			{
				Console.WriteLine (ex.ToString ());
			}
*/
		}


		protected override void OnPause()
		{
			base.OnPause ();
		}

		protected override void OnResume()
		{
			base.OnResume ();
		}
	}
}


