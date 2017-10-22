﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Class that can be used to toggle between to button profiles for any target component inheriting from ProfileButtonBase
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonToggle : MonoBehaviour
    {
        /// <summary>
        /// Toggle behavior
        /// </summary>
        public ToggleBehaviorEnum Behavior = ToggleBehaviorEnum.OnTapped;
        
        /// <summary>
        /// Profile to use when State is TRUE
        /// </summary>
        public ButtonProfile OnProfile;

        /// <summary>
        /// Profile to use when State is FALSE
        /// </summary>
        public ButtonProfile OffProfile;

        /// <summary>
        /// Component to target
        /// Must inherit from ProfileButtonBase
        /// </summary>
        public MonoBehaviour Target;

        /// <summary>
        /// Private reference of the compound button component
        /// </summary>
        private CompoundButton m_compButton;

        public bool State {
            get {
                return state;
            }
            set {
                SetState(value);
            }
        }

        [SerializeField]
        private bool state;

        /// <summary>
        /// On enable subscribe to button state change on the compound button component
        /// </summary>
        private void OnEnable() {
            m_compButton = GetComponent<CompoundButton>();

            // Force initial state setting
            SetState(state, true);

            if (m_compButton != null)
                m_compButton.StateChange += ButtonStateChange;
        }

        /// <summary>
        /// On disable unsubscribe to button state change on the compound button component
        /// </summary>
        private void OnDisable()
        {
            if (m_compButton != null)
                m_compButton.StateChange -= ButtonStateChange;
        }

        /// <summary>
        /// Handle button pressed callback from button
        /// </summary>
        /// <param name="buttonObj"></param>
        public void ButtonStateChange(ButtonStateEnum newState) {
            if(newState == ButtonStateEnum.Pressed)
            {
                switch (Behavior)
                {
                    default:
                        break;

                    case ToggleBehaviorEnum.OnTapped:
                        State = !State;
                        break;

                }
            }
            else if(newState == ButtonStateEnum.ObservationTargeted || newState == ButtonStateEnum.Targeted)
            {
                switch (Behavior)
                {
                    default:
                        break;

                    case ToggleBehaviorEnum.OnFocus:
                        State = !State;
                        break;
                }
            }
        }

        private void SetState (bool newState, bool force = false) {
            if ((!force || !Application.isPlaying) && state == newState)
                return;

            if (Target == null || OnProfile == null || OffProfile == null)
                return;

            state = newState;

            // Get the profile field of the target component and set it to the on profile
            // Store all icons in iconLookup via reflection
#if USE_WINRT
            FieldInfo fieldInfo = Target.GetType().GetTypeInfo().GetField("Profile");
#else
            FieldInfo fieldInfo = Target.GetType().GetField("Profile");
#endif
            if (fieldInfo == null) {
                Debug.LogError("Target component had no field type profile in CompoundButtonToggle");
                return;
            }

            fieldInfo.SetValue(Target, state ? OnProfile : OffProfile);

            if (Application.isPlaying) {
                // Disable, then re-enable the target
                // This will force the component to update itself
                Target.enabled = false;
                Target.enabled = true;
            }
        }        
    }
}
