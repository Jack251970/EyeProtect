using System;
using System.Collections.Generic;
using System.Linq;

namespace EyeProtect.Core.Core.Service
{
    public class ServiceCollection
    {
        /// <summary>
        /// 已创建的实例
        /// </summary>
        private readonly IList<object> instanceList;
        public ServiceCollection()
        {
            instanceList = new List<object>();
        }
        public void Add<T>() where T : IService
        {
            var type = typeof(T);
            CreateInstance(type);
        }
        public void AddInstance(object obj)
        {
            instanceList.Add(obj);
        }
        public void Initialize()
        {
            foreach (var instance in instanceList)
            {
                var method = instance.GetType().GetMethod("Init");
                method?.Invoke(instance, null);
            }
        }
        //public void AddViews()
        //{
        //    string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        //    string viewsNameSpace = assemblyName + ".Views";
        //    var viewTypeList = from t in Assembly.GetExecutingAssembly().GetTypes()
        //                       where t.IsClass && t.Namespace == viewsNameSpace
        //                       select t;
        //    foreach (var viewType in viewTypeList)
        //    {

        //    }
        //    //q.ToList().ForEach(t => Debug.WriteLine(t.Name));
        //}
        /// <summary>
        /// 创建实例
        /// </summary>
        private void CreateInstance(Type type)
        {
            var constructorInfoObj = type.GetConstructors()[0];
            var constructorParameters = constructorInfoObj.GetParameters();
            var constructorParametersLength = constructorParameters.Length;
            var types = new Type[constructorParametersLength];
            var objs = new object[constructorParametersLength];
            for (var i = 0; i < constructorParametersLength; i++)
            {
                var typeFullName = constructorParameters[i].ParameterType.FullName;
                var t = Type.GetType(typeFullName);
                types[i] = t;

                objs[i] = GetInstance(typeFullName);

            }
            var ctor = type.GetConstructor(types);
            var instance = ctor.Invoke(objs);
            if (instance != null)
            {
                instanceList.Add(instance);

            }
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public object GetInstance(string typeFullName)
        {
            var result = instanceList.Where(m => m.GetType().FullName == typeFullName);
            if (result.Count() > 0)
            {
                return result.Single();
            }
            return null;
        }
    }
}
