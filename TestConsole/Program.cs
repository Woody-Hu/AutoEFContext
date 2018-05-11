using AutoEFContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var useContxt = AutoContext.GetContext(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\aaa.mdf;Integrated Security=True"))
            {
                IDbSet<UserEntity> useDb = useContxt.GetDb<UserEntity>();

                useDb.Add(new UserEntity() { Name = "test" });

                var count = useContxt.SaveChanges();
            }
        }
    }


    [AutoEntity]
    public class UserEntity
    {
        public int UserEntityId { get; set; }
        public string Name { get; set; }
    }
}
