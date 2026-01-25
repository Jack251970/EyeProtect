namespace ProjectEye.Core.Models.Options
{
    public enum AnimationType
    {
        None = 0,
        RightBottomScale = 1,
        Opacity = 2,
        Cool = 3
    }

    public class AnimationModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 动画名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 动画类型
        /// </summary>
        public AnimationType AnimationType { get; set; }
    }
}
