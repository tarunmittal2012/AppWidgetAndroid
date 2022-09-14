using System;
using System.Collections.Generic;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Util;
using Android.Widget;

namespace AppWidget
{
	[BroadcastReceiver(Label = "HellApp Widget")]
	[IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	// The "Resource" file has to be all in lower caps
	[MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovide")]
	public class AppWidgetClass : AppWidgetProvider
	{
		private static string AnnouncementClick = "AnnouncementClickTag";

		/// <summary>
		/// This method is called when the 'updatePeriodMillis' from the AppwidgetProvider passes,
		/// or the user manually refreshes/resizes.
		/// </summary>
		public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidgetClass)).Name);
			appWidgetManager.UpdateAppWidget(me, BuildRemoteViews(context, appWidgetIds));
		}

		private RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds)
		{
			// Retrieve the widget layout. This is a RemoteViews, so we can't use 'FindViewById'
			var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

			SetTextViewText(widgetView);
			RegisterClicks(context, appWidgetIds, widgetView);
		//	SetPendingApprovalList(context,widgetView);
			return widgetView;
		}

        /*private void SetPendingApprovalList(Context context,RemoteViews widgetView)
        {
			String[] mobileArray = { "Approval 1", "Approval 2", "Approval 3" };
			Intent intent = new Intent(context, typeof(MyWidgetRemoteViewsService));
			widgetView.SetRemoteAdapter(Resource.Id.list,intent);
		}*/

        private void SetTextViewText(RemoteViews widgetView)
		{
			widgetView.SetTextViewText(Resource.Id.widgetMedium, "Approvals");
			widgetView.SetTextViewText(Resource.Id.widgetName, "Neeraj Kothari");
			widgetView.SetTextViewText(Resource.Id.widgetLive, "Live");
			widgetView.SetTextViewText(Resource.Id.widgetDate, "Wed 14 Sep");

		}

		private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
		{
			var intent = new Intent(context, typeof(AppWidgetClass));
			intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
			intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

			// Register click event for the Background
			var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
			widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);

			// Register click event for the Announcement-icon
			widgetView.SetOnClickPendingIntent(Resource.Id.widgetAnnouncementIcon, GetPendingSelfIntent(context, AnnouncementClick));
		}

		private PendingIntent GetPendingSelfIntent(Context context, string action)
		{
			var intent = new Intent(context, typeof(AppWidgetClass));
			intent.SetAction(action);
			return PendingIntent.GetBroadcast(context, 0, intent, 0);
		}

		/// <summary>
		/// This method is called when clicks are registered.
		/// </summary>
		public override void OnReceive(Context context, Intent intent)
		{
			base.OnReceive(context, intent);

			// Check if the click is from the "Announcement" button
			if (AnnouncementClick.Equals(intent.Action))
			{
				var pm = context.PackageManager;
				try
				{
					var packageName = "com.android.settings";
					var launchIntent = pm.GetLaunchIntentForPackage(packageName);
					context.StartActivity(launchIntent);
				}
				catch
				{
					// Something went wrong :)
				}
			}
		}
	}
}