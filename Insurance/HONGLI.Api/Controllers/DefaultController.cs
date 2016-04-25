using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HONGLI.Api.Models;
 
namespace HONGLI.Api.Controllers
{
    public class DefaultController : ApiController
    {
        public UserModel getuser()
        {
            return new UserModel() { Name = "guoqiangs", Age = 20 };
        }
 
    }
}
