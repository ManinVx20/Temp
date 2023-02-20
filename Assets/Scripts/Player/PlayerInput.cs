using UnityEngine;

namespace StormDreams
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MovementInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool SkillOneInput { get; private set; }
        public bool SkillTwoInput { get; private set; }

        private GameControls _gameControls;

        private void Awake()
        {
            _gameControls = new GameControls();
        }

        private void OnEnable()
        {
            _gameControls.Enable();
        }

        private void OnDisable()
        {
            _gameControls.Disable();
        }

        private void Update()
        {
            MovementInput = _gameControls.Player.Move.ReadValue<Vector2>();
            JumpInput = _gameControls.Player.Jump.triggered;
            SkillOneInput = _gameControls.Player.SkillOne.triggered;
            SkillTwoInput = _gameControls.Player.SkillTwo.triggered;
        }
    }
}
