namespace MyRoverServiceAPI
{
    public interface IMyRoversServiceGuard
    {
        /// <summary>
        /// Throws exception if the inputs are invalid.
        /// </summary>
        /// <param name="RoverName"></param>
        /// <param name="earthDay"></param>
       void GuardGetRoverImages(string RoverName, string earthDay);
    }
}