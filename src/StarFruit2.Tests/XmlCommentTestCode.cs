namespace StarFruit2.Tests
{
    /// <summary>
    /// Description for XmlCommentTestCode
    /// </summary>
    public class XmlCommentTestCode
    {
        /// <summary>
        /// Description for MyProperty
        /// </summary>
        public string MyProperty { get; }

        /// <summary>
        /// Description for MyMethod
        /// </summary>
        /// <param name="firstParam">Description for firstParam</param>
        /// <param name="secondParam">Description for secondParam</param>
        public void MyMethod(string firstParam, int secondParam) { }

        /// <summary>
        /// Description for MyEnum
        /// </summary>
        public enum MyEnum
        {
            /// <summary>
            /// Description for first
            /// </summary>
            first,

            /// <summary>
            /// Description for second
            /// </summary>
            /// 
            second,
            /// <summary>
            /// Description for third
            /// </summary>
            third
        }
    }
}
