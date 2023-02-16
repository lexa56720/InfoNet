using PsqlSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    class MainModel : BaseModel
    {
        public MainModel(ISqlApi api):base(api)
        {

        }

        public async Task Disconnect()
        {
            await Api.DisconnectAsync();
        }

    }
}
