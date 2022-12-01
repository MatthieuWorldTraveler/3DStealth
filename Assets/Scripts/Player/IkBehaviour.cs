using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkBehaviour : MonoBehaviour
{
	Animator _animator;
	Transform _lookAtTarget;

	[Range(0, 1f)]
	[SerializeField] float _groundDistance;
	[SerializeField] float _wallDistance;
	[SerializeField] LayerMask _playerMask;
	
	// Awake is called when the script instance is being loaded.
	protected void Awake()
	{
		_animator = GetComponent<Animator>();
		_lookAtTarget = Camera.main.transform;
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void OnAnimatorIK()
	{
		// IK code for Head movement
		_animator.SetLookAtWeight(1);
		Vector3 lookAt = _lookAtTarget.position + _lookAtTarget.forward * 20f;
		lookAt.y = 1;
        _animator.SetLookAtPosition(lookAt);

		// Ik for Foort movement
		// Left foot
		_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
		_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

		RaycastHit hit;
		Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
		if(Physics.Raycast(ray, out hit, _groundDistance + 1f, _playerMask))
		{
			if(hit.transform.tag == "Walkable")
			{
				Vector3 footPos = hit.point;
				footPos.y += _groundDistance;
				_animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
				_animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
			}
		}

        // Right foot
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

        ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, _groundDistance + 1f, _playerMask))
        {
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPos = hit.point;
                footPos.y += _groundDistance;
                _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }

        // Ik for Hand
        // Left Hand
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

         Ray raySide = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftHand) + Vector3.right, -transform.right);
		 Debug.DrawRay(_animator.GetIKPosition(AvatarIKGoal.LeftHand) + Vector3.right, -transform.right);
         Ray rayFront = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftHand) + Vector3.back, transform.forward);
         Debug.DrawRay(_animator.GetIKPosition(AvatarIKGoal.LeftHand) + Vector3.back, transform.forward);

        //if (Physics.Raycast(ray, out hit, _groundDistance + 1f, _playerMask))
        //{
        //    if (hit.transform.tag == "Touchable")
        //    {
        //        Vector3 footPos = hit.point;
        //        footPos.y += _groundDistance;
        //        _animator.SetIKPosition(AvatarIKGoal.LeftHand, footPos);
        //        _animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(transform.forward, hit.normal));
        //    }
        //}
    }
}
