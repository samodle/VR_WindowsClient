using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AWS;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeVR
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Variables & Properties
        private const string accessKey = "AKIAJ56HXAFX3LRSLLHQ";
        private const string secretKey = "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";

        private List<string> DynamoTableNames = new List<string>();
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
    
        }


        #region UI
        private void MainButtonA_Click(object sender, RoutedEventArgs e)
        {
        //   DynamoDB.CreateNewTableDemo("SteveBob");
            DynamoDB.InsertPeopleByDocumentModel("SteveBob");
        }

        private void MainButtonB_Click(object sender, RoutedEventArgs e)
        {
          //  DynamoDbDemoService service = new DynamoDbDemoService();
            DynamoDB.GetTablesDetails();
        }
        #endregion
    }
}
