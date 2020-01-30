﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILGenerator.Demo.FullFrameworkDemo
{
    class Program
    {
        static void Main(string[] args)
        {



            CustomTypeCreator ct = new CustomTypeCreator("testassembly", "test.dll");
            var cPerson = ct.CreateNewSimpleType("Person");
            cPerson.AddProperty("FirstName", typeof(string));
            cPerson.AddProperty("LastName", typeof(string));
            cPerson.AddProperty("Uid", typeof(Guid));

            var cContract = ct.CreateNewSimpleType("Contract");
            cContract.AddProperty("ContractNumber", typeof(string));
            cContract.AddProperty("Date", typeof(DateTime));
            cPerson.AddProperty("Contract", cContract.TypeBuilder);
            cPerson.AddPropertyOfGenericList("ConnectedPersons", cPerson.TypeBuilder);


            cPerson.AddPropertyGetAndSet();
            cContract.AddPropertyGetAndSet();

            var types = ct.Build();
            var tPerson = types.FirstOrDefault(w => w.Name == "Person");
            var tVertrag = types.FirstOrDefault(w => w.Name == "Contract");
            var iPerson = Activator.CreateInstance(tPerson, Array.Empty<object>());
            var iVertrag = Activator.CreateInstance(tVertrag, Array.Empty<object>());

            var wiPerson = (Interfaces.IPropertyGetAndSet)iPerson;
            var wiVertrag = (Interfaces.IPropertyGetAndSet)iVertrag;
            wiPerson.SetPropertyValue("FirstName", "Lukas");
            wiPerson.SetPropertyValue("LastName", "Dorn-Fussenegger");
            wiPerson.SetPropertyValue("Uid", Guid.NewGuid());
            wiPerson.SetPropertyValue("Contract", wiVertrag);

            wiVertrag.SetPropertyValue("ContractNumber", "1234-5678");
            wiVertrag.SetPropertyValue("Date", DateTime.Now);
            wiPerson.SetPropertyValue("ConnectedPersons", cPerson.BuildType.CreateInstanceOfList());
            for (int i = 0; i < 5; i++)
            {

                var li = wiPerson.GetPropertyValue("ConnectedPersons") as System.Collections.IList;

                var ci = tPerson.CreateInstanceWithPropertyGetAndSet();
                ci.SetPropertyValue("FirstName", $"Person{i}");
                ci.SetPropertyValue("Uid", Guid.NewGuid());
                li.Add(ci);

            }


            Console.WriteLine(
                Newtonsoft.Json.JsonConvert.SerializeObject(wiPerson, Newtonsoft.Json.Formatting.Indented)
                );

            Console.ReadKey();
        }
    }
}