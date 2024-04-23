using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Variable Declarations
	[Header("Reference")]
	[SerializeField] Canvas m_MainMenu;
	[SerializeField] Canvas m_GameplayMenu;
	[Space]
	[SerializeField] Transform m_Player;
	[SerializeField] Animator m_PlrAnimator;
	[SerializeField] Transform m_Planet;
	[Space]
	[SerializeField] Transform[] m_PickupPrefabs;
	[SerializeField] Transform[] m_DropPrefabs;

	[Header("Game Config")]
	[SerializeField] float m_InitialDelay;
	[SerializeField] float m_PickupSpawnDelay = 5;
	[SerializeField] float m_DropSpawnDelay = 5;
	[SerializeField] float m_SpawnLimit = 10;
	[Space]
	[SerializeField] float m_PlayerWalkSpeed;
	[SerializeField] float m_PlayerRunSpeed;
	[Space]
	[Tooltip("User to Spawn Pickups on Surface")]
	[SerializeField] float m_PlanetRadius;

	[Header("Runtime")]
	[SerializeField] bool m_IsPlaying;
	[SerializeField] float m_InitialTimer;
	[SerializeField] float m_PickupSpawnTimer;
	[SerializeField] float m_DropSpawnTimer;
	[SerializeField] List<Transform> m_Pickups;
	[SerializeField] List<Transform> m_Drops;
	#endregion

	#region Unity Callabacks
	private void Awake()
	{
		//m_IsPlaying = false;
		//m_GameplayMenu.enabled = false;

		//m_MainMenu.enabled = true;
	}

	private void Update()
	{
		if (!m_IsPlaying) return;
		ProcessInputs();

		if ((m_InitialTimer += Time.deltaTime) < m_InitialDelay) return;
		SpawnItems();
	}
	#endregion

	#region Private Functions
	private void ProcessInputs()
	{
		// We will do the Unit Speed to Angle Conversion later, (if really needed)
		var l_Speed = Input.GetKey(KeyCode.LeftShift) ? m_PlayerRunSpeed : m_PlayerWalkSpeed;
		l_Speed *= Time.deltaTime;

		var l_Axis = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) l_Axis += Vector3.left;
		if (Input.GetKey(KeyCode.S)) l_Axis += Vector3.right;
		if (Input.GetKey(KeyCode.A)) l_Axis += Vector3.back;
		if (Input.GetKey(KeyCode.D)) l_Axis += Vector3.forward;
		l_Axis.Normalize();
		m_Planet.Rotate(l_Axis, l_Speed, Space.World);

		var l_PlayerForword = Vector3.Cross(m_Player.up, l_Axis);
		m_Player.forward = l_PlayerForword;

		bool l_IsWalking = (l_Axis != Vector3.zero);
		m_PlrAnimator.SetBool("IsWalking", l_IsWalking);
	}
	private void SpawnItems()
	{
		if ((m_PickupSpawnTimer += Time.deltaTime) > m_PickupSpawnDelay)
		{
			m_PickupSpawnTimer -= m_PickupSpawnDelay;
			var l_Pickup = m_PickupPrefabs[Random.Range(0, m_PickupPrefabs.Length)];
			m_Pickups.Add(SpawnItemOnSurface(l_Pickup));
		}

		if ((m_DropSpawnTimer += Time.deltaTime) > m_DropSpawnDelay)
		{
			m_DropSpawnTimer -= m_DropSpawnDelay;
			var l_Drop = m_DropPrefabs[Random.Range(0, m_DropPrefabs.Length)];
			m_Drops.Add(SpawnItemOnSurface(l_Drop));
		}
	}
	#endregion

	#region Public Functions
	public Transform SpawnItemOnSurface(Transform a_Prototype)
	{
		var l_Trans = Instantiate(a_Prototype, m_Planet);
		l_Trans.position = Random.onUnitSphere * m_PlanetRadius;
		l_Trans.rotation = Quaternion.FromToRotation(Vector3.up, l_Trans.position); // Item's Down being towards Origin
		return l_Trans;
	}
	#endregion
}
