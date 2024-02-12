﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ModLoaderSolution
{
	public class SplitTimerText : MonoBehaviour
	{
		Color32 startingColor;
		public static SplitTimerText Instance { get; private set; }
		public Text text;
		public float time;
		public string checkpointTime = "";
		public bool count = false;
		bool uiEnabled = true;
		void Awake()
		{
			DontDestroyOnLoad(gameObject.transform.root);
			if (Instance != null && Instance != this)
				Destroy(this);
			else
				Instance = this;
		}
		public void CheckpointTime(string message)
        {
			StopCoroutine(DisableCheckpoint());
			checkpointTime = message;
			StartCoroutine(DisableCheckpoint());
		}
		IEnumerator DisableCheckpoint()
        {
			StopCoroutine("DisableCheckpoint");
			yield return new WaitForSeconds(5f);
			checkpointTime = "";
        }
		public IEnumerator DisableTimerText(float tim)
        {
			StopCoroutine("DisableTimerText");
			yield return new WaitForSeconds(tim);
			SetText("");
		}
		bool wasConnected = false;
		public void Update()
        {
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
			{
				UserInterface.Instance.SpecialNotif("UI Toggled: " + (!uiEnabled).ToString());
				uiEnabled = !uiEnabled;
				text.text = "";
			}
			if (!NetClient.Instance.IsConnected())
			{
				text.color = new Color32(247, 56, 42, 255);
				wasConnected = false;
			}
			else if (!wasConnected)
				TextColToDefault();
		}
		public void TextColToDefault()
        {
			text.color = startingColor;
        }
		void Start()
        {
			text = GetComponent<Text>();
			text.supportRichText = true;
			startingColor = text.color;
			SetText("");
			StartCoroutine(UpdateTime());
			GameObject.Find("TextShadow").GetComponent<Text>().supportRichText = true;
		}
		public void SetText(string textToSet)
        {
			textToSet = textToSet.Replace("\\n", "\n");
			if (uiEnabled)
				text.text = textToSet + "\n";
			else
				text.text = "";
		}
		public void RestartTimer()
		{
			time = 0;
			checkpointTime = "";
			count = true;
			text.color = startingColor;
		}
		public void StopTimer()
		{
			count = false;
			StartCoroutine(DisableTimerText(15));
		}
		public void FixedUpdate()
		{
			if (count)
				time += Time.deltaTime;				
		}
		IEnumerator UpdateTime()
        {
			while (true)
            {
				if (count)
					SetText(FormatTime(time).ToString() + "\n" + checkpointTime);
				yield return new WaitForEndOfFrame();
			}
		}
		public string FormatTime(float time)
		{
			int intTime = (int)time;
			int minutes = intTime / 60;
			int seconds = intTime % 60;
			float fraction = time * 100;
			fraction = (fraction % 100);
			string timeText = System.String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
			return timeText;
		}
	}
}