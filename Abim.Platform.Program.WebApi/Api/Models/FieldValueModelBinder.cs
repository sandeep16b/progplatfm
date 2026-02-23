using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

namespace Abim.Platform.Program.WebApi.Binding
{
    /// <summary>
    /// FieldValueModelBinder
    /// </summary>
    /// <seealso cref="System.Web.Http.ModelBinding.IModelBinder" />
    public class FieldValueModelBinder : IModelBinder
    {
        /// <summary>
        /// The numeric regex
        /// </summary>
        private const string RexChechNumeric = @"^\d+$";

        /// <summary>
        /// The brackets regex
        /// </summary>
        private const string RexBrackets = @"\[\d*\]";

        /// <summary>
        /// The search bracket regex
        /// </summary>
        private const string RexSearchBracket = @"\[([^}])\]";

        /// <summary>
        /// Define original source data list
        /// </summary>
        private List<KeyValuePair<string, string>> kvps;

        /// <summary>
        /// Set default maximum resursion limit
        /// </summary>
        private int maxRecursionLimit = 100;

        /// <summary>
        /// The recursion count
        /// </summary>
        private int recursionCount = 0;

        //Implement base member        
        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>
        /// true if model binding is successful; otherwise, false.
        /// </returns>
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            //Overwrite default maximum resursion limit if value set in config file
            var maxRecurseLimit = "100";
            if(!string.IsNullOrEmpty(maxRecurseLimit) && Regex.IsMatch(maxRecurseLimit, RexChechNumeric))
            {
                maxRecursionLimit = Convert.ToInt32(maxRecurseLimit);
            }

            //Check and get source data from uri 
            if(!string.IsNullOrEmpty(actionContext.Request.RequestUri.Query))
            {
                kvps = actionContext.Request.GetQueryNameValuePairs().Select(kvp => FixSwaggerPrependings(kvp)).ToList();
            }
            //Check and get source data from body 
            else if (actionContext.Request.Content.IsFormData())
            {
                var bodyString = actionContext.Request.Content.ReadAsStringAsync().Result;
                try
                {
                    kvps = ConvertToKvps(bodyString);
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                    return false;
                }
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No input data");
                return false;
            }
            //Initiate primary object
            var obj = Activator.CreateInstance(bindingContext.ModelType);
            try
            {
                //First call for processing primary object
                SetPropertyValues(obj);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, ex.Message);
                return false;
            }
            //Assign completed object tree to Model
            bindingContext.Model = obj;
            return true;
        }

        /// <summary>
        /// Sets the property values.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="parentObj">The parent object.</param>
        /// <param name="parentProp">The parent property.</param>
        public void SetPropertyValues(object obj, object parentObj = null, PropertyInfo parentProp = null)
        {
            //Recursively set PropertyInfo array for object hierarchy
            PropertyInfo[] props = obj.GetType().GetProperties();

            //Set KV Work List for real iteration process so that kvps is not in iteration and
            //its items from kvps can be removed after each iteration
            List<KeyValuePair<string, string>> kvpsWork;

            foreach (var prop in props)
            {
                //Refresh KV Work list from refreshed base kvps list after processing each property
                kvpsWork = new List<KeyValuePair<string, string>>(kvps);
                
                if((!prop.PropertyType.IsClass && !IsIList(prop.PropertyType)) || prop.PropertyType.FullName == "System.String")
                {
                    //For single or teminal properties.
                    foreach (var item in kvpsWork)
                    {
                        //Ignore any bracket in a name key 
                        var key = item.Key;
                        var keyParts = Regex.Split(key, RexBrackets);
                        if(keyParts.Length > 1) key = keyParts[keyParts.Length - 1];
                        if(PropertyNameMatch(key, prop, props, false))
                        {
                            //Populate KeyValueWork and pass it for adding property to object
                            var kvw = new KeyValueWork()
                            {
                                Key = item.Key,
                                Value = item.Value,
                                SourceKvp = item
                            };
                            AddSingleProperty(obj, prop, kvw);
                            break;
                        }
                    }
                }
                else if (prop.PropertyType.IsClass)
                {
                    //Check if List<string> or string[] type and assign string value directly to list or array item.    
                    if(prop.ToString().Contains("[System.String]") || prop.ToString().Contains("System.String[]"))
                    {
                        var strList = new List<string>();
                        foreach (var item in kvpsWork)
                        {
                            //Remove any brackets and enclosure from Key.
                            var itemKey = Regex.Replace(item.Key, RexBrackets, "");
                            if(PropertyNameMatch(itemKey, prop, props, false))
                            {
                                strList.Add(item.Value);
                                kvps.Remove(item);
                            }
                        }
                        //Add list to parent property.                        
                        if(prop.PropertyType.IsGenericType) prop.SetValue(obj, strList);
                        else if (prop.PropertyType.IsArray) prop.SetValue(obj, strList.ToArray());
                    }
                    else
                    {
                        //Check and process property encompassing complex object recursively
                        RecurseNestedObj(obj, prop);
                    }
                }
                else if (IsIList(prop.PropertyType))
                {
                    
                }
            }
            //Add property of this object to parent object 
            if(parentObj != null)
            {
                parentProp.SetValue(parentObj, obj, null);
            }
        }

        /// <summary>
        /// Determines whether a type is a List type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is i list] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIList(Type type)
        {
            return type.Name.Equals("IList`1");
        }

        /// <summary>
        /// Recurses the nested object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="prop">The property.</param>
        /// <param name="pParentName">Name of the p parent.</param>
        /// <param name="pParentObjIndex">Index of the p parent object.</param>
        /// <exception cref="System.Exception">
        /// Only support nested Generic List collection
        /// or
        /// Only support nested Generic List collection with one argument
        /// </exception>
        private void RecurseNestedObj(object obj, PropertyInfo prop, string pParentName = "", string pParentObjIndex = "")
        {
            //Check recursion limit
            if(recursionCount > maxRecursionLimit)
            {
                throw new Exception(string.Format("Exceed maximum recursion limit {0}", maxRecursionLimit));
            }
            recursionCount++;

            //Validate collection types
            if(prop.PropertyType.IsGenericType || prop.PropertyType.BaseType.IsGenericType)
            {
                if((prop.PropertyType.IsGenericType && prop.PropertyType.Name != "List`1")
                    || (prop.PropertyType.BaseType.IsGenericType && prop.PropertyType.BaseType.Name != "List`1"))
                {
                    throw new Exception("Only support nested Generic List collection");
                }
                if(prop.PropertyType.GenericTypeArguments.Count() > 1 || prop.PropertyType.BaseType.GenericTypeArguments.Count() > 1)
                {
                    throw new Exception("Only support nested Generic List collection with one argument");
                }
            }
            
            //Dynamically create instances for nested collection items
            object childObj = null;
            if(prop.PropertyType.IsGenericType || prop.PropertyType.BaseType.IsGenericType || prop.PropertyType.IsArray)
            {
                if(prop.PropertyType.IsGenericType)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
                }
                else if (!prop.PropertyType.IsGenericType && prop.PropertyType.BaseType.IsGenericType)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.BaseType.GenericTypeArguments[0]);
                }
                else if (prop.PropertyType.IsArray)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GetElementType());
                }
                //Call to process collection
                SetPropertyValuesForList(childObj, parentObj: obj, parentProp: prop,
                            pParentName: pParentName, pParentObjIndex: pParentObjIndex);
            }
            else
            {
                //Dynamically create instances for nested object and call to process it
                childObj = Activator.CreateInstance(prop.PropertyType);
                SetPropertyValues(childObj, parentObj: obj, parentProp: prop);
            }
        }

        /// <summary>
        /// Sets the property values for list.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="parentObj">The parent object.</param>
        /// <param name="parentProp">The parent property.</param>
        /// <param name="pParentName">Name of the p parent.</param>
        /// <param name="pParentObjIndex">Index of the p parent object.</param>
        private void SetPropertyValuesForList(object obj, object parentObj = null, PropertyInfo parentProp = null,
                                             string pParentName = "", string pParentObjIndex = "")
        {
            //Get props for type of object item in collection
            PropertyInfo[] props = obj.GetType().GetProperties();
            //KV Work For each object item in collection
            List<KeyValueWork> kvwsGroup = new List<KeyValueWork>();
            //KV Work for collection
            List<List<KeyValueWork>> kvwsGroups = new List<List<KeyValueWork>>();

            Regex regex;
            Match match;
            bool isGroupAdded = false;
            string lastIndex = "";
            
            foreach (var item in kvps)
            {
                //Passed parentObj and parentPropName are for List, whereas obj is instance of type for List
                if(item.Key.ToLower().Contains(parentProp.Name.ToLower()))
                {
                    //Get data only from parent-parent for linked child KV Work
                    if(pParentName != "" & pParentObjIndex != "")
                    {
                        regex = new Regex(pParentName + RexSearchBracket);      //don't just use RegexOptions.IgnoreCase, but use a smart case-insensitive check
                        match = regex.Match(item.Key);                          //... making sure there are no case-differing duplicates up the chain (currently not supported)
                        if(match.Groups[1].Value != pParentObjIndex)
                            break;
                    }
                    
                    //Get parts from current KV Work
                    regex = new Regex(parentProp.Name + RexSearchBracket);
                    match = regex.Match(item.Key);
                    var brackets = match.Value.Replace(parentProp.Name, "");
                    var objIdx = match.Groups[1].Value;
                    
                    //Point to start next idx and save last kvwsGroup data to kvwsGroups
                    if(lastIndex != "" && objIdx != lastIndex)
                    {
                        kvwsGroups.Add(kvwsGroup);
                        isGroupAdded = true;
                        kvwsGroup = new List<KeyValueWork>();
                    }
                    
                    //Get parts array from Key
                    var keyParts = item.Key.Split(new[] { brackets }, StringSplitOptions.RemoveEmptyEntries);
                    
                    //Populate KV Work
                    var kvw = new KeyValueWork()
                    {
                        ObjIndex = objIdx,
                        ParentName = parentProp.Name,
                        //Get last part from prefixed name
                        Key = keyParts[keyParts.Length - 1],
                        Value = item.Value,
                        SourceKvp = item
                    };
                    
                    //add KV Work to kvwsGroup list
                    kvwsGroup.Add(kvw);
                    lastIndex = objIdx;
                    isGroupAdded = false;
                }
            }
            
            //Handle the last kvwsgroup item if not added to final kvwsGroups List.
            if(kvwsGroup.Count > 0 && isGroupAdded == false)
                kvwsGroups.Add(kvwsGroup);

            //Initiate List or Array
            IList listObj = null;
            Array arrayObj = null;
            if(parentProp.PropertyType.IsGenericType || parentProp.PropertyType.BaseType.IsGenericType)
            {
                listObj = (IList)Activator.CreateInstance(parentProp.PropertyType);
            }
            else if (parentProp.PropertyType.IsArray)
            {
                arrayObj = Array.CreateInstance(parentProp.PropertyType.GetElementType(), kvwsGroups.Count);
            }

            int idx = 0;
            foreach (var group in kvwsGroups)
            {
                //Initiate object with type of collection item
                object tempObj = null;

                tempObj = Activator.CreateInstance(obj.GetType());
                //Iterate through properties of object model.
                foreach (var prop in props)
                {
                    if(!prop.PropertyType.IsClass || prop.PropertyType.FullName == "System.String")
                    {
                        //Assign terminal property to object
                        foreach (var item in group)
                        {
                            if(PropertyNameMatch(item.Key, prop, props, true))
                            {
                                AddSingleProperty(tempObj, prop, item);
                                break;
                            }
                        }
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        //Check if List<string> or string[] type and assign string value directly to list or array item.    
                        if(prop.ToString().Contains("[System.String]") || prop.ToString().Contains("System.String[]"))
                        {
                            //Match passed current processing object.
                            var tempProps = tempObj.GetType().GetProperties();
                            
                            //Iterate through current processing object properties.
                            foreach (var tempProp in tempProps)
                            {
                                if(tempProp.Name == prop.Name)
                                {
                                    var strList = new List<string>();
                                    
                                    //Iterate through passed data items.
                                    foreach (var item in group)
                                    {
                                        //Remove any brackets and enclosure from Key.
                                        var itemKey = Regex.Replace(item.Key, RexBrackets, "");
                                        if(PropertyNameMatch(itemKey, tempProp, tempProps, true))
                                        {
                                            strList.Add(item.Value);
                                            kvps.Remove(item.SourceKvp);
                                        }
                                    }
                                    //Add list to parent property.
                                    if(prop.PropertyType.IsGenericType) tempProp.SetValue(tempObj, strList);
                                    else if (prop.PropertyType.IsArray) tempProp.SetValue(tempObj, strList.ToArray());
                                }
                            }
                        }
                        //Check and process nested objects in collection recursively
                        //Pass ObjIndex for child KV Work items only for this parent object                        
                        else
                        {
                            RecurseNestedObj(tempObj, prop, pParentName: group[0].ParentName, pParentObjIndex: group[0].ObjIndex);
                        }
                    }
                }

                //Add populated object to List or Array                    
                if(listObj != null)
                {
                    listObj.Add(tempObj);
                }
                else if (arrayObj != null)
                {
                    arrayObj.SetValue(tempObj, idx);
                    idx++;
                }
            }
            //Add property for List or Array into parent object 
            if(listObj != null)
            {
                parentProp.SetValue(parentObj, listObj, null);
            }
            else if (arrayObj != null)
            {
                parentProp.SetValue(parentObj, arrayObj, null);
            }
        }

        /// <summary>
        /// Properties the name match.
        /// </summary>
        /// <param name="suppliedName">Name of the supplied.</param>
        /// <param name="prop">The property.</param>
        /// <param name="props">The props.</param>
        /// <param name="allowPrefixDot">if set to <c>true</c> [allow prefix dot].</param>
        /// <returns></returns>
        private bool PropertyNameMatch(string suppliedName, PropertyInfo prop, PropertyInfo[] props, bool allowPrefixDot)
        {
            if(allowPrefixDot && suppliedName.StartsWith("."))
                suppliedName = suppliedName.Substring(1);
            
            if(suppliedName == prop.Name) return true;
            
            var suppliedLower = suppliedName.ToLower();
            var anyCaseMatch = props.Where(p => p.Name.ToLower() == suppliedLower).ToList();
            if(anyCaseMatch.Count() == 1 && anyCaseMatch.First().Name == prop.Name) return true;
            
            return false;
        }

        /// <summary>
        /// Adds the single property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="prop">The property.</param>
        /// <param name="item">The item.</param>
        private void AddSingleProperty(object obj, PropertyInfo prop, KeyValueWork item)
        {
            if(prop.PropertyType.IsEnum)
            {
                var enumValues = prop.PropertyType.GetEnumValues();
                object enumValue = null;
                bool isFound = false;

                //Try to match enum item name first
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if(item.Value.ToLower() == enumValues.GetValue(i).ToString().ToLower())
                    {
                        enumValue = enumValues.GetValue(i);
                        isFound = true;
                        break;
                    }
                }
                //Try to match enum default underlying int value if not matched with enum item name
                if(!isFound)
                {
                    for (int i = 0; i < enumValues.Length; i++)
                    {
                        if(item.Value == i.ToString())
                        {
                            enumValue = i;
                            break;
                        }
                    }
                }
                prop.SetValue(obj, enumValue, null);
            }
            else
            {
                //Set value for non-enum terminal property 
                prop.SetValue(obj, Convert.ChangeType(item.Value, prop.PropertyType), null);
            }
            kvps.Remove(item.SourceKvp);
        }

        /// <summary>
        /// Converts to key-value pairs.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> ConvertToKvps(string sourceString)
        {
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            if(sourceString.StartsWith("?")) sourceString = sourceString.Substring(1);
            string[] elements = sourceString.Split('=', '&');
            for (int i = 0; i < elements.Length; i += 2)
            {
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>
                (
                    elements[i],
                    elements[i + 1]
                );
                kvpList.Add(kvp);
            }
            return kvpList;
        }

        /// <summary>
        /// This fixes an annoying swagger issue in which it's now been (as of spring 2017) prepending the top-level object name onto all
        /// query strings, e.g. "credentialComplexQuery.", or, for endpoints that don't take a complex query, "pageDefinition.". This does not
        /// break complex query binding, but does break page definition binding when the page definition is the top-level object. This method
        /// is a correction for that, which doesn't affect other cases because there's no scenario in which a user should actually start any
        /// query string with "pageDefinition."
        /// </summary>
        /// <param name="originalKvp">The original KVP.</param>
        /// <returns></returns>
        private static KeyValuePair<string, string> FixSwaggerPrependings(KeyValuePair<string, string> originalKvp)
        {
            string modifiedKey = originalKvp.Key;
            if(modifiedKey.ToLower().StartsWith("pagedefinition."))
                modifiedKey = modifiedKey.Substring("pagedefinition.".Length);
            var newKvp = new KeyValuePair<string, string>(modifiedKey, originalKvp.Value);
            return newKvp;
        }

        /// <summary>
        /// KeyValueWork class
        /// </summary>
        private class KeyValueWork
        {
            internal string ObjIndex { get; set; }
            internal string ParentName { get; set; }
            internal string Key { get; set; }
            internal string Value { get; set; }
            internal KeyValuePair<string, string> SourceKvp { get; set; }
        }
    }
}
