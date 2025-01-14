using App.GUI;
using App.Interaction;
using App.Settings;
using App.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Quests
{
	public class QuestProvider : InteractiveObject, INavigationItem
	{
		[LocalizeID]
		[SerializeField]
		private int nameID;

		[SerializeField]
		private int maxOffersCount = 3;

		[SerializeField]
		private QuestType questType;

		private QuestManager questManager;

		private PanelsManager panelsManager;

		private Animator animator;

		private Pauser pauser;

		private readonly int paramID = Animator.StringToHash("Gesture");

		private int animParam;

		private List<Quest> offer = new List<Quest>(8);

		private List<Quest> availableQuests = new List<Quest>(16);

		private List<Quest> questList = new List<Quest>(16);

		private Action<QuestProvider, List<Quest>> getQuestList;

		public List<Quest> Offer => offer;

		public int MaxOfferCount => maxOffersCount;

		public Quest ActiveQuest => questManager.CurrentQuest;

		public string Name
		{
			get;
			private set;
		}

		public QuestType QuestType => questType;

		Vector3 INavigationItem.Position => base.transform.position;

		bool INavigationItem.ShowOnMap => true;

		public void Initialize(QuestManager manager, Action<QuestProvider, List<Quest>> getQuestListCallback)
		{
			questManager = manager;
			getQuestList = getQuestListCallback;
			Initialize();
		}

		public void StartQuest(Quest quest)
		{
			if (!offer.Contains(quest))
			{
				UnityEngine.Debug.LogErrorFormat("Trying to start a Quest, which is not in the offer");
				return;
			}
			questManager.StartQuest(quest.Settings.ID);
			animParam = UnityEngine.Random.Range(1, 4);
		}

		public void AbandonQuest()
		{
			questManager.AbandonCurrentQuest();
		}

		public void OnOfferSelectionClose()
		{
			pauser.Resume();
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			pauser = ServiceLocator.Get<Pauser>();
			panelsManager = ServiceLocator.Get<PanelsManager>();
			animator = GetComponent<Animator>();
			Name = LocalizationManager.Instance.GetText(nameID);
			questManager.QuestChanged += OnQuestChanged;
		}

		protected override void OnInteract()
		{
			base.OnInteract();
			ShowOffer();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			animParam = 0;
		}

		private void OnQuestChanged(Quest quest)
		{
			if (quest.IsFinished)
			{
				offer.Remove(quest);
				availableQuests.Remove(quest);
				GenerateOffer();
			}
		}

		private bool CheckIsPlayer(Collider other)
		{
			WhoIsResult whoIsResult = WhoIs.Resolve(other, WhoIs.Masks.AllHumans);
			if (whoIsResult.IsEmpty)
			{
				return false;
			}
			return whoIsResult.gameObject.CompareTag("Player");
		}

		private void GenerateOffer()
		{
			int num = Mathf.Max(1, maxOffersCount);
			for (int i = 0; i < availableQuests.Count; i++)
			{
				if (offer.Count >= num)
				{
					break;
				}
				Quest item = availableQuests[i];
				if (!offer.Contains(item))
				{
					offer.Add(item);
				}
			}
		}

		private void ShowOffer()
		{
			LoadAvailableQuests();
			GenerateOffer();
			(pauser.PauseWithDialog(PanelType.QuestOffer) as QuestOfferPanel).UpdateData(this);
		}

		private void LoadAvailableQuests()
		{
			if (availableQuests.Count <= 0)
			{
				getQuestList(this, questList);
				foreach (Quest quest in questList)
				{
					if (!quest.IsFinished)
					{
						availableQuests.Add(quest);
					}
				}
			}
		}
	}
}
