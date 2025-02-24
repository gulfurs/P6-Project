using UnityEngine;
using UnityEngine.InputSystem;


	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move, look;
		public bool jump, sprint, crouch, attack, aim, interact;
		public bool equip1, equip2, equip3;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		
		//STANDARD MOVEMENT

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}
		
		//SPECIAL MOVEMENT

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnCrouch(InputValue value)
		{
			CrouchInput(value.isPressed);
		}

		//ATTACK

		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		// INTERACT

		public void OnInteract(InputValue value)
		{
		InteractInput(value.isPressed);
		}

	//EQUIP

		public void OnEquip1(InputValue value)
		{
		EquipInput(1, value.isPressed);
		}

		public void OnEquip2(InputValue value)
		{
		EquipInput(2, value.isPressed);
		}

		public void OnEquip3(InputValue value)
		{
		EquipInput(3, value.isPressed);
		}

		//INPUT
		
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void CrouchInput(bool newCrouchState)
		{
			crouch = newCrouchState;
		}

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

	public void EquipInput(int equipSlot, bool newState)
		{
			switch (equipSlot)
			{
			case 1:
				equip1 = newState;
				break;
			case 2:
				equip2 = newState;
				break;
			case 3:
				equip3 = newState;
				break;
			default:
				Debug.LogError("Invalid equipment slot");
				break;
			}
		}

	private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
