using System.Linq.Expressions;
using System.Reflection;

namespace Rugal.Net.DynamicSetter.Extention
{
    public static class ExpressionExtention
    {
        public static IEnumerable<MemberInfo> GetMembers(this LambdaExpression Exp)
        {
            var Members = new List<MemberInfo> { };
            if (Exp.Body is MemberExpression MemberExp)
            {
                Members.Add(MemberExp.Member);
            }
            else if (Exp.Body is NewExpression NewExp)
            {
                Members.AddRange(NewExp.Members);
            }
            else
            {
                throw new Exception("不支援的 Lambda 格式");
            }
            return Members;
        }


    }
}
