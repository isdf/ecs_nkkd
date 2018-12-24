﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public partial class ARIMotionSubWindow : EditorWindow
	{
		//パーツ選択の判定範囲
		const float SELECT_PARTS_SIZE = 1f;

		///<summary>入力イベント</summary>
		private void HandlingEvent()
		{
			//カメラ位置移動
			MoveCamera();
			//カメラ倍率変更
			ChangeCameraMag();

			SystemKey();

			//メインで選択中の項目
			switch (focusObject_)
			{
				//タック
				case enFocusObject.focusTack:

					switch (timelineType_)
					{
						case TimelineType.TL_POS:
							ShowMenu(new Dictionary<string, OnTrackEvent.EventType>
								{ { "Copy Pos", OnTrackEvent.EventType.EVENT_PARTS_COPY },
									{ "Paste Pos", OnTrackEvent.EventType.EVENT_PARTS_PASTE },
								});
							SelectParts(); //posは複数選択可能
							SelectPartsAll();
							SelectPartsKey();
							ChangePosKey();
							ChangePos();
							break;
						case TimelineType.TL_TRANSFORM:
							SelectParts();
							SelectPartsAll();
							ChangeTransformRotate();
							//ChangeTransformAngle();
							//ChangeTransformMirror();
							ChangeTransformReset();
							break;
						case TimelineType.TL_MOVE:
							break;
						case TimelineType.TL_COLOR:
							break;
						case TimelineType.TL_EFFECT:
							break;
						case TimelineType.TL_PASSIVE:
							break;
						default:
							Debug.Log("other timelineType");
							break;
					}

					break;
					//スコア
				case enFocusObject.focusScore:
					break;
			}
		}

		///<summary>カメラ視点移動</summary>
		private void MoveCamera()
		{
			Event e = Event.current;
			if (e.button != 2)return;
			if (e.type == EventType.MouseDown)
			{
				mouseStPos_ = e.mousePosition;
			}
			else if (e.type == EventType.MouseDrag) ///e.button 0:左ボタン、1:右ボタン、2:中ボタン
			{
				Vector2 dist = (e.mousePosition - mouseStPos_);
				camPos_ += (dist / mag_);
				mouseStPos_ = e.mousePosition;
				isRepaint_ = true;
			}
		}

		///<summary>カメラ倍率変更</summary>
		private void ChangeCameraMag()
		{
			Event e = Event.current;

			if (e.type == EventType.ScrollWheel)
			{
				int lastmag = mag_;
				bool isChange = false;
				if ((e.delta.y < 0) && (mag_ < MAXMAG))
				{
					mag_++;
					isChange = true;
				}
				else if ((e.delta.y > 0) && (mag_ > MINMAG))
				{
					mag_--;
					isChange = true;
				}

				if (isChange)
				{
					camPos_ = (camPos_ * lastmag) / mag_;
					isRepaint_ = true;
				}
			}
		}

		///<summary>右クリックによるメニュー表示</summary>
		private void ShowMenu(Dictionary<string, OnTrackEvent.EventType> menuItems)
		{
			// right click.	
			if (IsSelectedParts())return;
			if (Event.current.type == EventType.MouseDown) //クリック
			{
				if (Event.current.button == 1)ShowContextMenu(menuItems);
			}
		}
		///<summary>右クリックメニュー</summary>
		private void ShowContextMenu(Dictionary<string, OnTrackEvent.EventType> menuItems)
		{
			var menu = new GenericMenu();
			foreach (var key in menuItems.Keys)
			{
				var eventType = menuItems[key];
				menu.AddItem(new GUIContent(key), false, () => { Emit(new OnTrackEvent(eventType, null)); });
			}
			menu.ShowAsContext();
		}

		///<summary>システムキー入力</summary>
		private void SystemKey()
		{
			if (Event.current.type != EventType.KeyDown)return;

			if (Event.current.keyCode == KeyCode.Z) //Undo
			{
				ARIMotionMainWindow.tackCmd_.Undo();
				SetupPartsData(true);
			}
			else if (Event.current.keyCode == KeyCode.Y) //Redo
			{
				ARIMotionMainWindow.tackCmd_.Redo();
				SetupPartsData(true);
			}
			else if (Event.current.keyCode == KeyCode.S) //Save
			{
				parent_.SaveData2(true);
			}
			else if (Event.current.keyCode == KeyCode.L) //Load
			{
				parent_.ReloadSavedData();
			}
			else if (Event.current.keyCode == KeyCode.Q) //座標差分表示
			{
				isSabunPos_ = !isSabunPos_;
				isRepaint_ = true;
			}
		}

		///<summary>全パーツ選択</summary>
		private void SelectPartsAll()
		{
			if (Event.current.type != EventType.KeyDown)
				return;

			var keycode = Event.current.keyCode;

			if (keycode == KeyCode.A)
			{
				List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);
				bool isAllSelect = isMultiParts_.Where(p => p.Value == false).Any();

				foreach (var item in drawList)
				{
					isMultiParts_[item] = (isAllSelect)
						? true
						: !isMultiParts_[item];
					multiOffset_[item] = Vector2Int.zero;
				}

				isRepaint_ = true;
			}
		}

		///<summary>左クリックパーツ選択</summary>
		private void SelectParts()
		{
			if (Event.current.button != 0)
				return;

			Vector2 mousePos = (Event.current.mousePosition / mag_) - camPos_;
			//Yは反転
			mousePos.y = -mousePos.y;

			if (Event.current.type == EventType.MouseDown)
			{
				enPartsType partsType = enPartsType._END;
				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{

					Vector2 itemPos = GetPartsObject(item).pos;
					if ((mousePos.x > (itemPos.x - SELECT_PARTS_SIZE))
						&& (mousePos.x < (itemPos.x + SELECT_PARTS_SIZE))
						&& (mousePos.y > (itemPos.y - SELECT_PARTS_SIZE))
						&& (mousePos.y < (itemPos.y + SELECT_PARTS_SIZE))
					)
					{
						partsType = item;
						break;
					}
				}

				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{
					isMultiParts_[item] = (partsType == item);
					multiOffset_[item] = Vector2Int.zero;
				}
				isRepaint_ = true;

				SetupPartsData(true);
			}
		}

		///<summary>キーパーツ選択</summary>
		private void SelectPartsKey()
		{

			if (Event.current.type != EventType.KeyDown)
				return;

			var keycode = Event.current.keyCode;
			var isCtrl = Event.current.control;

			if ((keycode == KeyCode.Keypad1)
				|| (keycode == KeyCode.Keypad2)
				|| (keycode == KeyCode.Keypad3)
				|| (keycode == KeyCode.Keypad4)
				|| (keycode == KeyCode.Keypad5)
				|| (keycode == KeyCode.Keypad6)
				|| (keycode == KeyCode.Keypad7)
				|| (keycode == KeyCode.Keypad8)
				|| (keycode == KeyCode.Keypad9)
				|| (keycode == KeyCode.Keypad0)
				|| (keycode == KeyCode.KeypadPeriod)
			)
			{
				List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);
				enPartsType partsType = enPartsType.Body;
				switch (keycode)
				{
					case KeyCode.Keypad8:
						partsType = enPartsType.Ant;
						break;
					case KeyCode.Keypad5:
						partsType = enPartsType.Head;
						break;
					case KeyCode.Keypad2:
						partsType = enPartsType.Body;
						break;
					case KeyCode.Keypad7:
						partsType = enPartsType.RightArm;
						break;
					case KeyCode.Keypad4:
						partsType = enPartsType.RightHand;
						break;
					case KeyCode.Keypad9:
						partsType = enPartsType.LeftArm;
						break;
					case KeyCode.Keypad6:
						partsType = enPartsType.LeftHand;
						break;
					case KeyCode.Keypad1:
						partsType = enPartsType.RightLeg;
						break;
					case KeyCode.Keypad0:
						partsType = enPartsType.RightFoot;
						break;
					case KeyCode.Keypad3:
						partsType = enPartsType.LeftLeg;
						break;
					case KeyCode.KeypadPeriod:
						partsType = enPartsType.LeftFoot;
						break;
					default:
						return;
				}

				if (isCtrl)
				{
					foreach (var item in drawList)
					{
						if (partsType != item)
							continue;

						isMultiParts_[item] = !isMultiParts_[item];
						multiOffset_[item] = Vector2Int.zero;
					}
				}
				else
				{
					foreach (var item in drawList)
					{
						isMultiParts_[item] = (partsType != item)
							? false
							: !isMultiParts_[item];
						multiOffset_[item] = Vector2Int.zero;
					}
				}

				isRepaint_ = true;
			}
		}

		//Pos---------------

		///<summary>マウスによる位置移動</summary>
		private void ChangePos()
		{
			if (Event.current.button != 0)return;

			if (!IsSelectedParts())return;

			Vector2 mousePos = (Event.current.mousePosition / mag_);
			if (Event.current.type == EventType.MouseDrag) //ドラッグ
			{
				List<Action> cmdDo = new List<Action>();
				List<Action> cmdUndo = new List<Action>();
				string id = MethodBase.GetCurrentMethod().Name;
				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{
					if (!isMultiParts_[item])continue;

					Vector2 movePos = new Vector2(
						mousePos.x - camPos_.x - multiOffset_[item].x, -mousePos.y + camPos_.y - multiOffset_[item].y);
					Undo.RecordObject(parent_, "ChangePartsPos");

					Vector2Int newPos = GetNewPos(item, RoundPosVector(movePos));
					var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
					var lastPos = activeTack.motionData_.mPos.GetPos(item);
					var partsType = item;
					id += partsType;
					//コマンドPos
					cmdDo.Add(() => activeTack.motionData_.mPos.SetPos(partsType, newPos));
					cmdUndo.Add(() => activeTack.motionData_.mPos.SetPos(partsType, lastPos));
				}

				if (cmdDo.Any())
				{

					ARIMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
						() => { foreach (var cmd in cmdDo)cmd(); },
						() => { foreach (var cmd in cmdUndo)cmd(); }));
				}

				SetupPartsData(true);
			}
		}

		///<summary>キーによる位置移動</summary>
		private void ChangePosKey()
		{
			if (Event.current.type != EventType.KeyDown)return;

			var keycode = Event.current.keyCode;

			if ((keycode == KeyCode.UpArrow) || (keycode == KeyCode.DownArrow)
				|| (keycode == KeyCode.LeftArrow) || (keycode == KeyCode.RightArrow)
				|| (keycode == KeyCode.KeypadMinus))
			{

				Undo.RecordObject(parent_, "ChangePartsPosKey");

				List<Action> cmdDo = new List<Action>();
				List<Action> cmdUndo = new List<Action>();
				string id = MethodBase.GetCurrentMethod().Name;

				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{
					if (!isMultiParts_[item])continue;

					Vector2Int movePos = GetPartsObject(item).pos;
					switch (keycode)
					{
						case KeyCode.UpArrow:
							movePos.y += 1;
							break;
						case KeyCode.DownArrow:
							movePos.y -= 1;
							break;
						case KeyCode.LeftArrow:
							movePos.x -= 1;
							break;
						case KeyCode.RightArrow:
							movePos.x += 1;
							break;
						case KeyCode.KeypadMinus:
							movePos = BasePosition.GetPosEdit(item, false);
							break; //元の位置に戻す
					}

					Vector2Int newPos = GetNewPos(item, RoundPosVector(movePos));
					var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
					var lastPos = activeTack.motionData_.mPos.GetPos(item);
					var partsType = item;
					//コマンドPos
					cmdDo.Add(() => activeTack.motionData_.mPos.SetPos(partsType, newPos));
					cmdUndo.Add(() => activeTack.motionData_.mPos.SetPos(partsType, lastPos));
					id += partsType;
				}

				if (cmdDo.Any())
				{
					ARIMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
						() => { foreach (var cmd in cmdDo)cmd(); },
						() => { foreach (var cmd in cmdUndo)cmd(); }));
				}

				SetupPartsData(true);
			}
		}

		//アクティブタックの位置変更
		private Vector2Int GetNewPos(enPartsType partsType, Vector2Int pos)
		{
			Vector2Int newPos = Vector2Int.zero;
			Vector2Int basePos = BasePosition.GetPosEdit(partsType, false);
			newPos.x = pos.x - basePos.x;
			newPos.y = pos.y - basePos.y;

			//地面にもぐらないように
			//float GROUNDY = 8;

			int MAXPOSX = 48;
			int MAXPOSY = 64;
			if (newPos.y + basePos.y < 0)newPos.y = (-basePos.y);
			//if (newPos.y + basePos.y < BasePosition.GROUNDY) newPos.y = (BasePosition.GROUNDY - basePos.y);
			if (newPos.y + basePos.y > MAXPOSY)newPos.y = MAXPOSY - basePos.y;
			if (newPos.x + basePos.x < -MAXPOSX)newPos.x = -MAXPOSX - basePos.x;
			if (newPos.x + basePos.x > MAXPOSX)newPos.x = MAXPOSX - basePos.x;

			return newPos;
		}

		//Transform---------------
		private void ChangeTransformRotate()
		{
			if (Event.current.type != EventType.KeyDown)return;

			if (!IsSelectedParts())return;

			var keycode = Event.current.keyCode;

			if ((keycode == KeyCode.Keypad4) || (keycode == KeyCode.Keypad6))
			{
				List<Action> cmdDo = new List<Action>();
				List<Action> cmdUndo = new List<Action>();

				string id = MethodBase.GetCurrentMethod().Name;

				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{
					if (!isMultiParts_[item])continue;

					int r = (int)GetPartsObject(item).partsTransform.rotate;
					enPartsRotate newRotate = enPartsRotate.Rotate0;
					switch (keycode)
					{
						case KeyCode.Keypad4:
							newRotate = (enPartsRotate)((r + 360 - 90) % 360);
							break;
						case KeyCode.Keypad6:
							newRotate = (enPartsRotate)((r + 90) % 360);
							break;
					}

					Undo.RecordObject(parent_, "ChangePartsRotate");

					var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
					activeTack.motionData_.mTransform.SetRotate(item, newRotate);
					var lastRotate = activeTack.motionData_.mTransform.GetRotate(item);
					var partsType = item;
					id += partsType;
					cmdDo.Add(() => activeTack.motionData_.mTransform.SetRotate(partsType, newRotate));
					cmdUndo.Add(() => activeTack.motionData_.mTransform.SetRotate(partsType, lastRotate));
				}

				if (cmdDo.Any())
				{
					ARIMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
						() => { foreach (var cmd in cmdDo)cmd(); },
						() => { foreach (var cmd in cmdUndo)cmd(); }));
				}

				SetupPartsData(true);
			}
		}

		////パーツ向き
		//private void ChangeTransformAngle()
		//{
		//	if (Event.current.type != EventType.KeyDown) return;

		//	if (!IsSelectedParts()) return;

		//	var keycode = Event.current.keyCode;

		//	if ((keycode == KeyCode.Keypad1) || (keycode == KeyCode.Keypad2) || (keycode == KeyCode.Keypad3)
		//		|| (keycode == KeyCode.Keypad4) || (keycode == KeyCode.Keypad5))
		//	{
		//		enPartsAngle newAngle = enPartsAngle.Side;
		//		switch (keycode)
		//		{
		//			case KeyCode.Keypad1: newAngle = enPartsAngle.Front; break;
		//			case KeyCode.Keypad2: newAngle = enPartsAngle.Side; break;
		//			case KeyCode.Keypad3: newAngle = enPartsAngle.Rear; break;
		//			case KeyCode.Keypad4: newAngle = enPartsAngle.Look; break;
		//			case KeyCode.Keypad5: newAngle = enPartsAngle.Back; break;
		//		}

		//		Undo.RecordObject(parent_, "ChangePartsAngle");

		//		var activeTack = parent_.GetActiveScore().GetActiveTackPoint();

		//		List<Action> cmdDo = new List<Action>();
		//		List<Action> cmdUndo = new List<Action>();
		//		string id = MethodBase.GetCurrentMethod().Name;

		//		foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
		//		{
		//			if (!isMultiParts_[item]) continue;
		//			var lastAngle = activeTack.motionData_.mTransform.GetAngle(item);
		//			var partsType = item;
		//			id += partsType;
		//			cmdDo.Add(() => activeTack.motionData_.mTransform.SetAngle(partsType, newAngle));
		//			cmdUndo.Add(() => activeTack.motionData_.mTransform.SetAngle(partsType, lastAngle));
		//		}

		//		//コマンドTransform
		//		if (cmdDo.Any())
		//		{
		//			JMMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
		//				() => { foreach (var cmd in cmdDo) cmd(); },
		//				() => { foreach (var cmd in cmdUndo) cmd(); }));
		//		}

		//		SetupPartsData(true);
		//	}
		//}

		////パーツ反転
		//private void ChangeTransformMirror()
		//{
		//	if (Event.current.type != EventType.KeyDown) return;

		//	if (!IsSelectedParts()) return;

		//	var keycode = Event.current.keyCode;

		//	if (keycode == KeyCode.Keypad6)
		//	{
		//		List<Action> cmdDo = new List<Action>();
		//		List<Action> cmdUndo = new List<Action>();
		//		string id = MethodBase.GetCurrentMethod().Name;

		//		foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
		//		{
		//			if (!isMultiParts_[item]) continue;

		//			bool newMirror = !(GetPartsObject(item).partsTransform.mirror);

		//			Undo.RecordObject(parent_, "ChangePartsMirror");

		//			var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
		//			var lastMirror = activeTack.motionData_.mTransform.GetMirror(item);
		//			var partsType = item;
		//			id += partsType;

		//			cmdDo.Add(() => activeTack.motionData_.mTransform.SetMirror(partsType, newMirror));
		//			cmdUndo.Add(() => activeTack.motionData_.mTransform.SetMirror(partsType, lastMirror));
		//		}

		//		//コマンドTransform
		//		if (cmdDo.Any())
		//		{
		//			JMMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
		//				() => { foreach (var cmd in cmdDo) cmd(); },
		//				() => { foreach (var cmd in cmdUndo) cmd(); }));
		//		}

		//		SetupPartsData(true);
		//	}
		//}

		//リセット
		private void ChangeTransformReset()
		{
			if (Event.current.type != EventType.KeyDown)return;

			if (!IsSelectedParts())return;

			var keycode = Event.current.keyCode;

			if (keycode == KeyCode.KeypadMinus)
			{
				Undo.RecordObject(parent_, "ChangePartsResetTransform");
				var activeTack = parent_.GetActiveScore().GetActiveTackPoint();

				List<Action> cmdDo = new List<Action>();
				List<Action> cmdUndo = new List<Action>();
				string id = MethodBase.GetCurrentMethod().Name;

				foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
				{
					if (!isMultiParts_[item])continue;
					var lastTransform = activeTack.motionData_.mTransform.GetTransform(item);
					var partsType = item;
					id += partsType;

					cmdDo.Add(() => activeTack.motionData_.mTransform.Reset(partsType));
					cmdUndo.Add(() => activeTack.motionData_.mTransform.SetTransform(partsType, lastTransform));
				}

				//コマンドTransform
				if (cmdDo.Any())
				{
					ARIMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
						() => { foreach (var cmd in cmdDo)cmd(); },
						() => { foreach (var cmd in cmdUndo)cmd(); }));
				}

				SetupPartsData(true);
			}
		}
	}
}