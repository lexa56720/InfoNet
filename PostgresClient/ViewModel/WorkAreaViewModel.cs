using PostgresClient.Model;
using PsqlSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.ViewModel
{
    class WorkAreaViewModel: BaseViewModel
    {
        protected override WorkAreaModel Model { get=>(WorkAreaModel)base.Model; }


        public WorkAreaViewModel(ISqlApi api):base(api) 
        {
        }
        protected override BaseModel CreateModel(ISqlApi api)
        {
            return new WorkAreaModel(api);
        }

    }
}
