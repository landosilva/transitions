namespace Lando.Patterns.Transitions
{
    public static class Transition
    {
        public static void To(string sceneName)
        {
            ITransitionView defaultView = TransitionsSettings.GetDefaultTransitionView();
            To(sceneName, defaultView);
        }

        public static void To(string sceneName, ITransitionView transitionView)
        {
            TransitionHandler transitionHandler = new TransitionHandler(transitionView);
            _ = transitionHandler.StartTransition(sceneName);
        }
    }
}
