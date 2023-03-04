﻿using PsqlSharp;
using System;
using System.Threading.Tasks;

namespace PostgresClient.Model
{
    internal class FuncsModel : BaseModel
    {
        private Function[]? Functions { get; set; }
        public FuncsModel(ISqlApi api) : base(api)
        {
        }

        public async Task<string[]?> GetFunctionsHeader()
        {
            Functions = await Api.GetAllFunctions();

            return Functions != null ? Array.ConvertAll(Functions, x => x.ToString()) : null;
        }

        public async Task<string> GetFunctionCode(int index)
        {
            if (index < Functions.Length && index >= 0)
            {
                if (Functions == null)
                    await GetFunctionsHeader();

                return Functions[index].UserCode;
            }
            return string.Empty;
        }

        public async Task<bool> UpdateFunction(int index, string code)
        {
            if (index < Functions.Length && index >= 0)
            {
                Functions[index].UserCode = code;
                return await Api.UpdateFunction(Functions[index]);
            }
            return false;
        }

        public async Task DeleteFunction(int index)
        {
            if (index<Functions.Length && index>=0 )
                await Api.RemoveFunction(Functions[index]);
        }
    }
}
