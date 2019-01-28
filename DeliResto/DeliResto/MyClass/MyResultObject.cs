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
  class MyResultObject
  {
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string Result { get; set; }
  }
}