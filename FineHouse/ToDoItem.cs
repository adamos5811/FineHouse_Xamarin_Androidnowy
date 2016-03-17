using System;
using Newtonsoft.Json;


namespace FineHouse
{
	public class ToDoItem
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "complete")]
		public bool Complete { get; set; }
	}

    public class CreateUserBindingModel
    {

        public string Id { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "firstNae")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }



        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "Confirmpassword")]
        public string ConfirmPassword { get; set; }
    }

    public class ToDoItemWrapper : Java.Lang.Object
	{
		public ToDoItemWrapper (ToDoItem item)
		{
			ToDoItem = item;
		}

		public ToDoItem ToDoItem { get; private set; }

	}
}

