using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TopicEngine.Services
{
	public class ServiceProvider
	{
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}

		public static void ToObject()
		{
			try
			{
				Console.WriteLine("noexception");
				// output = JsonConvert.DeserializeObject<>();
				// return output;
			}
			catch (Exception e)
			{
				Console.WriteLine("Caught an Exception");
				Console.WriteLine(e.Message);
                // return new Object();
			}
		}
	}
}