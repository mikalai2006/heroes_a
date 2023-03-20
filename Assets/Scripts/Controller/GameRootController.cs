//using UnityEngine;

///// <summary>
///// Root controller responsible for changing game phases with SubControllers.
///// </summary>
//public class GameRootController : MonoBehaviour
//{
//    // SubControllers types.
//    public enum ControllerTypeEnum
//    {
//        Aside,
//        Town,
//        Setting
//    }

//    // References to the subcontrollers.
//    [Header("Controllers")]
//    [SerializeField]
//    private GameAsideController asideController;
//    //[SerializeField]
//    //private GameController gameController;
//    //[SerializeField]
//    //private GameOverController gameOverController;

//    /// <summary>
//    /// Unity method called on first frame.
//    /// </summary>
//    private void Start()
//    {
//        //menuController.root = this;
//        asideController.root = this;
//        //gameOverController.root = this;

//        ChangeController(ControllerTypeEnum.Menu);
//    }

//    /// <summary>
//    /// Method used by subcontrollers to change game phase.
//    /// </summary>
//    /// <param name="controller">Controller type.</param>
//    public void ChangeController(ControllerTypeEnum controller)
//    {
//        // Reseting subcontrollers.
//        DisengageControllers();

//        // Enabling subcontroller based on type.
//        switch (controller)
//        {
//            case ControllerTypeEnum.Menu:
//                menuController.EngageController();
//                break;
//            case ControllerTypeEnum.Game:
//                gameController.EngageController();
//                break;
//            //case ControllerTypeEnum.GameOver:
//            //    gameOverController.EngageController();
//            //    break;
//            default:
//                break;
//        }
//    }

//    /// <summary>
//    /// Method used to disable all attached subcontrollers.
//    /// </summary>
//    public void DisengageControllers()
//    {
//        menuController.DisengageController();
//        gameController.DisengageController();
//        //gameOverController.DisengageController();
//    }
//}
