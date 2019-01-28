using Android.App;
using Android.Widget;
using Android.OS;
using DeliResto.MyClass;
using System.Threading;
using System;
using System.Diagnostics;
using Android.Views.InputMethods;
using Android.Content;
using System.Collections.Generic;

namespace DeliResto
{
  [Activity(Label = "DeliResto", MainLauncher = true, Theme = "@android:style/Theme.DeviceDefault.Light.NoActionBar")]
  public class LoginActivity : Activity
  {
    EditText usernameTxt, passwordTxt;
    Button loginBtn;
    ProgressBar mProgress;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.LoginView);

      usernameTxt = FindViewById<EditText>(Resource.Id.usernameTxt);
      passwordTxt = FindViewById<EditText>(Resource.Id.passwordTxt);
      loginBtn = FindViewById<Button>(Resource.Id.loginBtn);
      mProgress = FindViewById<ProgressBar>(Resource.Id.progressBar);

      loginBtn.Click += LoginBtn_Click;
    }

    private void LoginBtn_Click(object sender, System.EventArgs e)
    {
      string username, password ;
      username = usernameTxt.Text;
      password = passwordTxt.Text;

      hideKeyboard(usernameTxt);
      hideKeyboard(passwordTxt);

      mProgress.Visibility = Android.Views.ViewStates.Visible;
      mProgress.Progress = 50;
      try
      {
        new Thread(new ThreadStart(delegate
        {
          RunOnUiThread(() =>
          {
            Connection myConnection = new Connection();
            List<MyObjectInJson> loginResult = myConnection.LoginProcess(username, password);
            
            if(loginResult!=null)
            {
              if(loginResult.Count>0)
              {
                if (loginResult[1].ObjectInJson.ToString() == "")
                {
                  if (loginResult[0].ObjectID == "key")
                  {
                    if (loginResult[0].ObjectInJson.ToString() == "1")
                    {
                      Dictionary<string, string> userData = (Dictionary<string, string>)loginResult[2].ObjectInJson;
                      Session.name = userData["name"];
                      Session.username = userData["username"];
                      Session.userLevel = int.Parse(userData["level"]);

                      Intent serviceIntent = new Intent(this, typeof(TableMenuActivity));
                      
                      ////To pass:
                      //intent.putExtra("Myitem", item);

                      //// To retrieve object in second Activity
                      //getIntent().getSerializableExtra("Myitem");

                      this.StartActivity(serviceIntent);
                    }
                    else
                    {
                      Toast.MakeText(this, loginResult[1].ObjectInJson.ToString(), ToastLength.Short).Show();
                    }
                  }
                  else
                  {
                    Toast.MakeText(this, "No key index found", ToastLength.Short).Show();
                  }
                }
                else
                {
                  Toast.MakeText(this, loginResult[1].ObjectInJson.ToString(), ToastLength.Short).Show();
                }
              }
              else
              {
                Toast.MakeText(this, "No data returned", ToastLength.Short).Show();
              }
            }
            else
            {
              Toast.MakeText(this, "Unknown error occured", ToastLength.Short).Show();
            }

            mProgress.Progress = 100;
            mProgress.Visibility = Android.Views.ViewStates.Invisible;
          });
        })).Start();
      }
      catch(Exception ex)
      {
        Toast.MakeText(this, ErrorMessage(ex.Message), ToastLength.Short).Show();
        mProgress.Progress = 0;
        mProgress.Visibility = Android.Views.ViewStates.Invisible;
      }
    }

    public void hideKeyboard(EditText editText)
    {
      InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
      imm.HideSoftInputFromWindow(editText.WindowToken, 0);
    }

    public string ErrorMessage(string eMessage)
    {
      //+PIO 20190108 get class and method name
      var st = new StackTrace();
      var sf = st.GetFrame(0);

      var currentMethodName = sf.GetMethod();

      return "Class: " + this.GetType().Name + "\nMethod: " + currentMethodName + "\nError: " + eMessage;
    }
  }
}

