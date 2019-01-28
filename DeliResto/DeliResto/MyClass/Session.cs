using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DeliResto.MyClass
{
  class Session
  {
    public static string name;
    public static string username { get; set; }
    public static int userLevel { get; set; }

    public static bool checkSession(Activity currActivity)
    {
      if(Session.username!=null && Session.name!=null)
      {
        if(Session.username!="" && Session.name!="")
        {
          return true;
        }
      }

      Intent intent = new Intent(currActivity, typeof(LoginActivity));
      currActivity.StartActivity(intent);

      return false;
    }
  }
}