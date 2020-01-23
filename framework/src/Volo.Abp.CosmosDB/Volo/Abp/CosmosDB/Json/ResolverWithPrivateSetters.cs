using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Volo.Abp.CosmosDB.Json
{
    // https://talkdotnet.wordpress.com/2019/03/15/newtonsoft-json-deserializing-objects-that-have-private-setters/
    public class ResolverWithPrivateSetters : DefaultContractResolver
    {
        public ResolverWithPrivateSetters()
        {
            NamingStrategy = new CamelCaseNamingStrategy();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable)
            {
                return prop;
            }

            prop.Writable = member.As<PropertyInfo>()?.GetSetMethod(true) != null;

            return prop;
        }
    }
}