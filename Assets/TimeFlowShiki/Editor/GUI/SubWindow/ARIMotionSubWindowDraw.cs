using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public partial class ARIMotionSubWindow
	{

		//描画
		void DrawAutoConponent()
		{
			DrawLabel();

			DrawChar();
			if (isRepaint_)Repaint();
		}

		//描画系///////////////////////////////
		void DrawLabel()
		{

			if (focusObject_ == enFocusObject.focusTack)
			{
				EditorGUILayout.LabelField(timelineType_.ToString());
				EditorGUILayout.LabelField("S：保存");
				EditorGUILayout.LabelField("L：読み込み");
				EditorGUILayout.LabelField("Z：やり直し");
				EditorGUILayout.LabelField("Y：元に戻す");
				EditorGUILayout.LabelField("A：全パーツ選択");
				EditorGUILayout.LabelField("Q：座標切替");

				switch (timelineType_)
				{
					case TimelineType.TL_POS:
						EditorGUILayout.LabelField("左クリック：パーツ選択");
						EditorGUILayout.LabelField("右クリック：パーツ選択解除");
						EditorGUILayout.LabelField("上下左右：パーツ移動");
						EditorGUILayout.LabelField("-：位置リセット");
						break;
					case TimelineType.TL_TRANSFORM:
						EditorGUILayout.LabelField("左クリック：パーツ選択");
						EditorGUILayout.LabelField("右クリック：パーツ選択解除");
						EditorGUILayout.LabelField("12345：向き前横後");
						EditorGUILayout.LabelField("79：角度");
						EditorGUILayout.LabelField("8：反転");
						EditorGUILayout.LabelField("-：リセット");
						break;
					case TimelineType.TL_MOVE:
						break;
					case TimelineType.TL_ATARI:
						EditorGUILayout.LabelField("左クリック：当たり判定ON／OFF");
						EditorGUILayout.LabelField("右クリック：当たり判定OFF");
						break;
					case TimelineType.TL_HOLD:
						EditorGUILayout.LabelField("上下左右：移動");
						EditorGUILayout.LabelField("79：角度");
						EditorGUILayout.LabelField("8：反転");
						EditorGUILayout.LabelField("-：リセット");
						break;
					case TimelineType.TL_THROW:
						break;
					case TimelineType.TL_COLOR:
						break;
					case TimelineType.TL_EFFECT:
						break;
					case TimelineType.TL_PASSIVE:
						break;
				}
			}

		}
		// グリッド線を描画
		void DrawGridLine3()
		{
			// grid
			Handles.color = new Color(1f, 1f, 1f, 0.3f);

			//縦線
			{
				Vector2 st = new Vector2(0, -64);
				Vector2 ed = new Vector2(0, 0);
				Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
			}

			//横線
			{
				Vector2 st = new Vector2(-64, 0);
				Vector2 ed = new Vector2(64, 0);
				Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
			}

		}
		void DrawGridLine3Sub()
		{
			for (int i = 0; i < 8; i++)
			{
				//縦線
				{
					Vector2 st = new Vector2(i * 8, -64);
					Vector2 ed = new Vector2(i * 8, 0);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}

				//縦線
				{
					Vector2 st = new Vector2(i * -8, -64);
					Vector2 ed = new Vector2(i * -8, 0);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}

				//横線
				{
					Vector2 st = new Vector2(-64, i * -8);
					Vector2 ed = new Vector2(64, i * -8);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}
			}
		}

		// グリッド線を描画
		void DrawGridLine3Cross(Vector2 pos, Color col)
		{
			if (pos == Vector2.zero)return;
			// grid
			Handles.color = col;

			//縦線
			{
				Vector2 st = new Vector2(0, -32);
				Vector2 ed = new Vector2(0, 32);
				Handles.DrawLine((camPos_ + st + pos) * mag_, (camPos_ + ed + pos) * mag_);
			}

			//横線
			{
				Vector2 st = new Vector2(-32, 0);
				Vector2 ed = new Vector2(32, 0);
				Handles.DrawLine((camPos_ + st + pos) * mag_, (camPos_ + ed + pos) * mag_);
			}
		}

		//全パーツ描画
		void DrawPartsAll()
		{
			try
			{
				List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);

				if (focusObject_ == enFocusObject.focusTack)
				{
					foreach (var item in drawList)
						if (!isMultiParts_[item])DrawParts(item, false); //非選択
					//foreach (var item in drawList) if (isMultiParts_[item]) DrawParts(item, isHold, false);//選択
				}
				else
				{
					foreach (var item in drawList)DrawParts(item, false);
				}
			}
			catch
			{

			}

		}

		bool IsDark(enPartsType partsType)
		{
			bool res = false;
			if (focusObject_ == enFocusObject.focusTack)
			{
				var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
				switch (timelineType_)
				{
					case TimelineType.TL_POS:
					case TimelineType.TL_TRANSFORM:
						res = (isMultiParts_[partsType] == false);
						break;
					case TimelineType.TL_MOVE:
						break;
						//case TimelineType.TL_ATARI:
						//	res = !activeTack.motionData_.mAtari.IsAtari(partsType);
						//	break;
						//case TimelineType.TL_HOLD:
						//	res = !isHold;
						//	break;
					case TimelineType.TL_COLOR:
						res = !activeTack.motionData_.mColor.IsActive(partsType);
						break;
					case TimelineType.TL_EFFECT:
						break;
					case TimelineType.TL_PASSIVE:
						break;
				}
			}
			return res;
		}

		//パーツ描画
		void DrawParts(enPartsType partsType, bool isLabel)
		{
			PartsObject partsObject = GetPartsObject(partsType);

			Vector2 pos = partsObject.pos;

			//if (partsType == enPartsType.Head) {
			//	Debug.Log(pos);
			//}
			pos.y = -pos.y; //上下反転
			//bool isLeft = partsObject.partsTransform.isLeft;

			Sprite sp = parent_.GetSprite(partsType, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);
			if (sp != null)
			{
				Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
				Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

				Vector2 drawPos = Vector2.zero;
				int MAG = mag_;
				if (isLabel)
				{
					MAG = 5;
					Vector2 labelpos = new Vector2(128, 256) / MAG;
					drawPos = (basepos + labelpos + pos);
				}
				else
				{
					drawPos = (basepos + camPos_ + pos + tempMovePos_);
				}

				Rect drawRect = new Rect(drawPos * MAG, size * MAG);
				if (sendMotion_.stPassive.isLeft)
				{
					drawRect.x += drawRect.width;
					drawRect.width = -drawRect.width;
				}
				Vector2 rotatePivot = new Vector2(drawRect.center.x, drawRect.center.y);
				float rotate = partsObject.partsTransform.rotate;

				if ((partsType == enPartsType.LeftArm)
					|| (partsType == enPartsType.LeftLeg))
				{
					GUI.color = (isLabel || IsDark(partsType))
						? new Color(0.75f, 0.5f, 0.5f)
						: new Color(1, 0.8f, 0.8f);
				}
				else
				{
					GUI.color = (isLabel || IsDark(partsType))
						? new Color(0.5f, 0.5f, 0.5f)
						: new Color(1, 1, 1);
				}

				GUIUtility.RotateAroundPivot(-rotate, rotatePivot);
				GUI.DrawTextureWithTexCoords(drawRect, sp.texture, GetSpriteNormalRect(sp)); //描画

				// RotateAroundPivot等は行列の掛け算なので、一旦初期値に戻す
				GUI.matrix = Matrix4x4.identity;
				GUI.color = new Color(1, 1, 1);
			}

		}

		void DrawPartsLabel()
		{
			List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);
			foreach (var item in drawList)DrawParts(item, false);

			const int MAG = 5;

			foreach (var item in drawList)
			{
				PartsObject partsObject = GetPartsObject(item);

				Vector2 pos = partsObject.pos;
				pos.y = -pos.y; //上下反転
				//bool mirror = partsObject.partsTransform.mirror;
				Sprite sp = parent_.GetSprite(item, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);

				if (sp == null)break;

				Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
				//Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

				Vector2 labelpos = new Vector2(128, 256) / MAG;
				Vector2 drawPos = (basepos + labelpos + pos);
				// GUIの見た目を変える。
				GUIStyle guiStyle = new GUIStyle();
				GUIStyleState styleState = new GUIStyleState();

				string vecstr = "";
				if (isSabunPos_)
				{
					// テキストの色を設定
					styleState.textColor = Color.yellow;
					vecstr = "(" + ((int)(sendMotion_.stPos.GetPos(item).x)).ToString() + "," + ((int)(sendMotion_.stPos.GetPos(item).y)).ToString() + ")";
				}
				else
				{
					// テキストの色を設定
					styleState.textColor = Color.white;
					vecstr = "(" + ((int)(drawPos.x)).ToString() + "," + ((int)(drawPos.y)).ToString() + ")";
				}

				// スタイルの設定。
				guiStyle.normal = styleState;
				guiStyle.alignment = TextAnchor.MiddleCenter;

				Vector2 labelPos = new Vector2(drawPos.x, drawPos.y);
				Rect labelRect = new Rect(labelPos * MAG, new Vector2(sp.rect.width, sp.rect.height) * MAG);

				GUI.Label(labelRect, vecstr, guiStyle);
			}

		}

		//キャラ描画
		void DrawChar()
		{
			DrawPartsAll();
			//DrawPartsAll(false);
			// DrawPartsLabel();

			DrawGridLine3();
			if (focusObject_ == enFocusObject.focusTack)
			{
				DrawGridLine3Sub();
				DrawGridLine3Cross(tempMovePos_, new Color(1f, 0.5f, 1f, 0.8f));
			}
		}

		//スプライトの大きさ取得
		public static Rect GetSpriteNormalRect(Sprite sp)
		{
			// spriteの親テクスチャー上のRect座標を取得.
			Rect rectPosition = sp.textureRect;

			// 親テクスチャーの大きさを取得.
			float parentWith = sp.texture.width;
			float parentHeight = sp.texture.height;
			// spriteの座標を親テクスチャーに合わせて正規化.
			Rect NormalRect = new Rect(
				rectPosition.x / parentWith,
				rectPosition.y / parentHeight,
				rectPosition.width / parentWith,
				rectPosition.height / parentHeight
			);

			return NormalRect;
		}

	}
}