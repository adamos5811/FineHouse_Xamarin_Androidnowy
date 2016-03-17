using System;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.IO;

namespace FineHouse
{
    [Activity(MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/AppTheme")]
    public class ToDoActivity : Activity
    {
        //Mobile Service Client reference
        private MobileServiceClient client;

        //Mobile Service sync table used to access data
        private IMobileServiceSyncTable<ToDoItem> toDoTable;
        private IMobileServiceSyncTable<CreateUserBindingModel> User;


        //Adapter to map the items list to the view
        private ToDoItemAdapter adapter;

        //EditText containing the "New ToDo" text
        private EditText textNewToDo;
        private EditText email;
        private EditText password;
        private EditText confirm;
        private EditText firstname;
        private EditText lastname;
        private EditText user;



        const string applicationURL = @"http://localhost:51287";        

        const string localDbFilename = "localstore.db";

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Activity_To_Do);

            CurrentPlatform.Init();

            // Create the Mobile Service Client instance, using the provided
            // Mobile Service URL
            client = new MobileServiceClient(applicationURL);
           await InitLocalStoreAsync();

            // Get the Mobile Service sync table instance to use
            //toDoTable = client.GetSyncTable<ToDoItem>();
           User = client.GetSyncTable<CreateUserBindingModel>();


            textNewToDo = FindViewById<EditText>(Resource.Id.textNewToDo);
            email = FindViewById<EditText>(Resource.Id.email);
            password = FindViewById<EditText>(Resource.Id.password);
            confirm = FindViewById<EditText>(Resource.Id.confirm);
            firstname = FindViewById<EditText>(Resource.Id.Firstname);
            lastname = FindViewById<EditText>(Resource.Id.lastname);
            user = FindViewById<EditText>(Resource.Id.User);







            // Create an adapter to bind the items with the view
            adapter = new ToDoItemAdapter(this, Resource.Layout.Row_List_To_Do);

           // var listViewToDo = FindViewById<ListView>(Resource.Id.listViewToDo);
           // listViewToDo.Adapter = adapter;

            // Load the items from the Mobile Service
            OnRefreshItemsSelected();

        }

        private async Task InitLocalStoreAsync()
        {
            // new code to initialize the SQLite store
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), localDbFilename);

            if (!File.Exists(path)) {
                File.Create(path).Dispose();
            }

            var store = new MobileServiceSQLiteStore(path);
     store.DefineTable<CreateUserBindingModel>();

            // Uses the default conflict handler, which fails on conflict
            // To use a different conflict handler, pass a parameter to InitializeAsync. For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
            await client.SyncContext.InitializeAsync(store);
        }

        //Initializes the activity menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_main, menu);
            return true;
        }
        /*
        //Select an option from the menu
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_refresh) {
                item.SetEnabled(false);

                OnRefreshItemsSelected();

                item.SetEnabled(true);
            }
            return true;
        }
        */
        private async Task SyncAsync(bool pullData = false)
        {
            try {
                await client.SyncContext.PushAsync();

                if (pullData) {
                  //  await toDoTable.PullAsync("allTodoItems", toDoTable.CreateQuery()); // query ID is used for incremental sync
                    await User.PullAsync("CreateUser", User.CreateQuery());
                }
            }
            catch (Java.Net.MalformedURLException) {
                CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
        }

        // Called when the refresh menu option is selected
        private async void OnRefreshItemsSelected()
        {
            await SyncAsync(pullData: true); // get changes from the mobile service
        }

        //Refresh the list with the items in the local database
       

        public async Task CheckItem(CreateUserBindingModel item)
        {
            if (client == null) {
                return;
            }

            // Set the item as completed and update it in the table
            
                await SyncAsync(); // send changes to the mobile service

              
            
        }

        [Java.Interop.Export()]
        public async void AddItem(View view)
        {
          //  if (client == null || string.IsNullOrWhiteSpace(textNewToDo.Text)) {
            //    return;
            //}

            // Create a new item
            var item = new ToDoItem {
                Text = textNewToDo.Text,
                Complete = false
                
            };

            var user = new CreateUserBindingModel
            {
                Email = this.email.Text,
                Username = this.user.Text,
                Password = this.password.Text,
                ConfirmPassword = this.confirm.Text,
                FirstName = this.firstname.Text,
                LastName = this.lastname.Text

            };
            

            try {
              //  await toDoTable.InsertAsync(item); // insert the new item into the local database
                await SyncAsync(true); // send changes to the mobile service

                if (!item.Complete) {
                    adapter.Add(user);
                }
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }

            textNewToDo.Text = "";
        }

        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}


