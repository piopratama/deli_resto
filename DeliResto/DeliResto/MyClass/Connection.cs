using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace DeliResto.MyClass
{
  class Connection
  {
    const int SUCCESS = 1;
    const int NOERROR = 0;
    const int ERROR = -1;
    const string IP = "192.168.1.9";

    private HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + IP + "/github/deli_resto_service/service.php");

    public List<MyObjectInJson> GetTableRestaurant()
    {
      List<MyObjectInJson> listData = new List<MyObjectInJson>();
      List<MyObjectInJson> listDataResult = null;

      MyObjectInJson myObjectJson = new MyObjectInJson();

      try
      {
        myObjectJson.ObjectID = "url";
        myObjectJson.ObjectInJson = "tablemenu";

        listData.Add(myObjectJson);

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "username";
        myObjectJson.ObjectInJson = Session.username;

        listData.Add(myObjectJson);

        var myResult = GetJSONData(ListObjectToJson(listData));

        if (myResult.ErrorCode == 0)
        {
          if (myResult.Result.Length > 0)
          {
            listDataResult = JsonConvert.DeserializeObject<List<MyObjectInJson>>(myResult.Result);

            if (listDataResult.Count > 2)
            {
              int length_data_json = listDataResult[2].ObjectInJson.ToString().Length;
              string data_json = listDataResult[2].ObjectInJson.ToString().Substring(1, length_data_json - 2);
              var x = JsonConvert.DeserializeObject<Dictionary<string, string>>(data_json);
              listDataResult[2].ObjectInJson = x;
            }
          }
        }
        else if (myResult.ErrorCode == -1)
        {
          listDataResult = new List<MyObjectInJson>();

          myObjectJson = new MyObjectInJson();
          myObjectJson.ObjectID = "key";
          myObjectJson.ObjectID = "-1";
          listDataResult.Add(myObjectJson);

          myObjectJson = new MyObjectInJson();
          myObjectJson.ObjectID = "message";
          myObjectJson.ObjectID = myResult.ErrorMessage;
          listDataResult.Add(myObjectJson);
        }

        return listDataResult;
      }
      catch(Exception e)
      {
        listDataResult = new List<MyObjectInJson>();

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "key";
        myObjectJson.ObjectID = "-1";
        listDataResult.Add(myObjectJson);

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "message";
        myObjectJson.ObjectID = ErrorMessage(e.Message);
        listDataResult.Add(myObjectJson);

        return listDataResult;
      }
    }

    public List<MyObjectInJson> LoginProcess(string username, string password)
    {
      List<MyObjectInJson> listData = new List<MyObjectInJson>();
      List<MyObjectInJson> listDataResult = null;

      MyObjectInJson myObjectJson = new MyObjectInJson();

      try
      {
        myObjectJson.ObjectID = "url";
        myObjectJson.ObjectInJson = "login";

        listData.Add(myObjectJson);

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "username";
        myObjectJson.ObjectInJson = username;

        listData.Add(myObjectJson);

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "password";
        myObjectJson.ObjectInJson = password;

        listData.Add(myObjectJson);


        var myResult = GetJSONData(ListObjectToJson(listData));

        if (myResult.ErrorCode == 0)
        {
          if (myResult.Result.Length > 0)
          {
            listDataResult = JsonConvert.DeserializeObject<List<MyObjectInJson>>(myResult.Result);

            if (listDataResult.Count > 2)
            {
              int length_data_json = listDataResult[2].ObjectInJson.ToString().Length;
              string data_json = listDataResult[2].ObjectInJson.ToString().Substring(1, length_data_json - 2);
              var x = JsonConvert.DeserializeObject<Dictionary<string, string>>(data_json);
              listDataResult[2].ObjectInJson = x;
            }
          }
        }
        else if(myResult.ErrorCode==-1)
        {
          listDataResult = new List<MyObjectInJson>();

          myObjectJson = new MyObjectInJson();
          myObjectJson.ObjectID = "key";
          myObjectJson.ObjectID = "-1";
          listDataResult.Add(myObjectJson);

          myObjectJson = new MyObjectInJson();
          myObjectJson.ObjectID = "message";
          myObjectJson.ObjectID = myResult.ErrorMessage;
          listDataResult.Add(myObjectJson);
        }

        return listDataResult;
      }
      catch(Exception e)
      {
        listDataResult = new List<MyObjectInJson>();

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "key";
        myObjectJson.ObjectID = "-1";
        listDataResult.Add(myObjectJson);

        myObjectJson = new MyObjectInJson();
        myObjectJson.ObjectID = "message";
        myObjectJson.ObjectID = ErrorMessage(e.Message);
        listDataResult.Add(myObjectJson);

        return listDataResult;
      }
    }

    private List<MyObjectInJson> JsonToListObject(String json)
    {
      return JsonConvert.DeserializeObject<List<MyObjectInJson>>(json);
    }

    private string ListObjectToJson(List<MyObjectInJson> listData)
    {
      return JsonConvert.SerializeObject(listData);
    }

    private MyResultObject GetJSONData(string json)
    {
      try
      {
        //+PIO 20190108 pass and get json data from the server
        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = json.Length;
        request.Timeout = 5000;

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
          streamWriter.Write(json);
          streamWriter.Close();

          var httpResponse = (HttpWebResponse)request.GetResponse();
          using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();

            MyResultObject myResultObject = new MyResultObject();
            myResultObject.ErrorCode = NOERROR;
            myResultObject.ErrorMessage = "";
            myResultObject.Result = result;

            return myResultObject;
          }
        }
      }
      catch(Exception e)
      {
        MyResultObject myResultObject = new MyResultObject();
        myResultObject.ErrorCode = ERROR;
        myResultObject.ErrorMessage = ErrorMessage(e.Message);
        myResultObject.Result = "";

        return myResultObject;
      }
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