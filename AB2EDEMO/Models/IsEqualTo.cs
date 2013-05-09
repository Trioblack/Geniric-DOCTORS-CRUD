using System;

namespace AB2EDEMO.Models
{
    public class Check
    {
        public static bool AreEqual(object object1, object object2)
        {
            var result = true;
            if (object1.GetType() != object2.GetType())
            {
                throw new ArgumentException("Parameter must be of the same type");
            }
            foreach (var property in object1.GetType().GetProperties())
            {
                if (property.GetValue(object1,null).ToString().TrimEnd()!=property.GetValue(object2,null).ToString().TrimEnd())
                {
                    result = false;
                }
            }
            return result;
        }
    }

}