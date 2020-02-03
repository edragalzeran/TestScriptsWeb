using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using TestScriptsWeb.Client.Pages;

namespace TestScriptsWeb.Client
{
    public class AuscultacioApi
    {
        private readonly LocalStorageService _localStorage;

        public string Host
        {
            get => _host;
            set
            {
                _host = value;

                _debugUri = "http://" + _host + ":15344/debug";

                //_localStorage.SetItem("AuscultacioHost", _host);
            }
        }

        public AuscultacioApi(LocalStorageService localStorage)
        {
            _localStorage = localStorage;
            var host = localStorage.GetItem<string>("AuscultacioHost");

            if (String.IsNullOrEmpty(host))
                _host = "http://localhost";
            else _host = host;

            _debugUri = _host + ":15344/debug";
        }

        public string _debugUri;

        private string _host="http://localhost";
    }


    public class Variable
    {
        public int id;
        public string codi;
        public int idProjecte;
        public string RowClass = null;
    }

    public class Valor:IComparer<Valor>
    {
        public long idValor;
        public DateTime dataHoraValor;
        public string valor;
        public string observacions;
        public ETipusEstatValor tipusEstatValor;
        public ETipusValor tipusValor;
        public Alerta[] alertes;

        public string RowClass
        {
            get
            {
                if (alertes != null && alertes.Length > 0)
                {
                    return "table-danger";
                }

                return null;
            }
        }

        public int Compare(Valor x, Valor y)
        {
            if (x.idValor > y.idValor)
                return 1;
            if (x.idValor < y.idValor)
                return -1;
            else
                return 0;
        }
    }

    public class Alerta
    {

    }

    public enum ETipusEstatValor
    {
        Normal = 1,
        Falta_Valor = 2,
        Fallo_Calculo = 3,
        Consolidado = 4,
        Eliminado = 5
    }

    public enum ETipusValor
    {
        Manual = 1,
        Manual_importado = 2,
        Automatico = 3,
        Automatico_solicitado = 4,
        Automatico_calculado = 5,
        Recalculado = 6,
        Automatico_importado = 7,
        Automatico_importado_calculado = 8,
        Manual_importado_calculado = 9,
        Automatico_estimado = 10,
    }
}
