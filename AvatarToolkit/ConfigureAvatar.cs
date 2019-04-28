////////////////////////////////////////////
///
// Pierrce - Avatar Toolkit
// (c) 2019 Pierrce
//
////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using VRCSDK2;
using VRC.Core;
using System.Collections.Generic;

namespace Pierrce.Toolkits.VRC
{
    [ExecuteInEditMode, CanEditMultipleObjects]
    public class ConfigureAvatar : EditorWindow
    {
        public GameObject avatar;
        public GameObject transferAvatar;

        public VRC_AvatarDescriptor.AnimationSet animationSet;

        public AnimatorOverrideController standing;
        public AnimatorOverrideController sitting;

        public string status;
        public string objectCount;
        public string meshCount;
        public string skinCount;
        public string triangles;
        public string materials;
        public string shaders;
        public string colliders;
        public string dynamicBones;
        public string dynamicColliders;
        public string dynamicColliderAffectedTransforms;
        public string particleSystems;
        public string maxParticles;
        public string rigidbodies;

        public bool dynamicBoneState = false;
        public bool avatarTransferState = false;
        public bool vrcState = false;
        public bool statsState = false;
        public bool animatorState = false;

        public Vector2 scrollPos;

        [MenuItem("Pierrce/Toolkits/Avatar Toolkit")]
        public static void ShowFuckOffWindow()
        {
            EditorWindow.GetWindow(typeof(ConfigureAvatar), false, "VRChat Toolkit");
        }

        public void OnGUI()
        {
            GUILayout.Label("Pierrce's VRChat Toolkit", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            GUILayout.Space(20f);
            avatar = (GameObject)EditorGUILayout.ObjectField("Avatar", avatar, typeof(GameObject), true);
            transferAvatar = (GameObject)EditorGUILayout.ObjectField("Transfer Avatar", transferAvatar, typeof(GameObject), true);
            animationSet = (VRC_AvatarDescriptor.AnimationSet)EditorGUILayout.EnumPopup("Gender", VRC_AvatarDescriptor.AnimationSet.Female);

            GUILayout.Space(5f);
            animatorState = EditorGUILayout.Foldout(animatorState, "Custom Animators", true, EditorStyles.foldout);
            if (animatorState)
            {
                standing = (AnimatorOverrideController)EditorGUILayout.ObjectField("Standing", standing, typeof(AnimatorOverrideController), true);
                sitting = (AnimatorOverrideController)EditorGUILayout.ObjectField("Sitting", sitting, typeof(AnimatorOverrideController), true);
            }

            GUILayout.Space(5f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Assign Avatar(s)"))
            {
                if(avatar && transferAvatar)
                {
                    var v = avatar;
                    avatar = transferAvatar;
                    transferAvatar = v;
                }
                else
                {
                    avatar = GameObject.FindObjectsOfType<VRC_AvatarDescriptor>()[0].gameObject ?? GameObject.FindObjectsOfType<Animator>()[0].gameObject;
                    transferAvatar = GameObject.FindObjectsOfType<VRC_AvatarDescriptor>()[1].gameObject ?? GameObject.FindObjectsOfType<Animator>()[1].gameObject;
                }
                
                status = "Avatars have been set!";
            }

            if (GUILayout.Button("Initialize Avatar"))
            {
                if (avatar)
                    AddComponents();
                else
                    status = "Please set the avatar.";
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);
            avatarTransferState = EditorGUILayout.Foldout(avatarTransferState, "Avatar Transfer Tools", true, EditorStyles.foldout);
            if (avatarTransferState)
            {
                GUILayout.Label("Copy", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Everything"))
                {

                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Missing Objects"))
                {
                    if (avatar && transferAvatar)
                        CopyMissingObjectsOver(avatar, transferAvatar);
                    else
                        status = "Please set the avatar and transfer avatar.";
                }
                if (GUILayout.Button("Dynamic Bones"))
                {
                    
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Colliders"))
                {

                }
                if (GUILayout.Button("VRC Scripts"))
                {

                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Move", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("Everything"))
                {

                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Missing Objects"))
                {
                    if (avatar && transferAvatar)
                        MoveMissingObjectsOver(avatar, transferAvatar);
                    else
                        status = "Please set the avatar and transfer avatar.";
                }
                if (GUILayout.Button("Dynamic Bones"))
                {

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Colliders"))
                {

                }
                if (GUILayout.Button("VRC Scripts"))
                {

                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
            dynamicBoneState = EditorGUILayout.Foldout(dynamicBoneState, "Dynamic Bone Tools", true, EditorStyles.foldout);
            if (dynamicBoneState)
            {
                GUILayout.Label("Transfer", EditorStyles.miniBoldLabel);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Main To Children"))
                {
                    if (avatar)
                        MainToChildren();
                    else
                        status = "Please set the avatar.";
                }

                if (GUILayout.Button("Children To Main"))
                {
                    if (avatar)
                        ChildrenToMain();
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Remove", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("All Bones"))
                {
                    if (avatar)
                        RemoveAllBones();
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Children Bones"))
                {
                    if (avatar)
                        FuckOffDynamicChildren();
                    else
                        status = "Please set the avatar.";
                }

                if (GUILayout.Button("Main Bones"))
                {
                    if (avatar)
                        FuckOffMainBones();
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Hair"))
                {
                    var hair = GetAnimator(avatar).GetBoneTransform(HumanBodyBones.Head).GetComponent<DynamicBone>();
                    DestroyImmediate(hair);
                }

                if (GUILayout.Button("Chest"))
                {
                    var chest = GetAnimator(avatar).GetBoneTransform(HumanBodyBones.Chest).GetComponent<DynamicBone>();
                    DestroyImmediate(chest);
                }

                if (GUILayout.Button("Hips"))
                {
                    var hips = GetAnimator(avatar).GetBoneTransform(HumanBodyBones.Hips).GetComponent<DynamicBone>();
                    DestroyImmediate(hips);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Add", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("All Bones"))
                {
                    PopulateDynamicBones(GetAnimator(avatar));
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Hair"))
                {
                    SetHeadDynamicBones(GetAnimator(avatar));
                }

                if (GUILayout.Button("Chest"))
                {
                    SetChestDynamicBones(GetAnimator(avatar));
                }

                if (GUILayout.Button("Hips"))
                {
                    SetHipDynamicBones(GetAnimator(avatar));
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
            vrcState = EditorGUILayout.Foldout(vrcState, "VRC Tools", true, EditorStyles.foldout);
            if (vrcState)
            {
                GUILayout.Label("Add", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("All Scripts"))
                {
                    if (avatar)
                        PopulateVRChat(GetDescriptor(avatar));
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Avatar Descriptor"))
                {
                    if (avatar)
                        avatar.GetOrAddComponent<VRC_AvatarDescriptor>();
                    else
                        status = "Please set the avatar.";
                }
                if (GUILayout.Button("Pipeline Manager"))
                {
                    if (avatar)
                        avatar.GetOrAddComponent<PipelineManager>();
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Set", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("All Scripts"))
                {
                    PopulateVRChat(GetDescriptor(avatar));
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Viewpoint"))
                {
                    if (avatar)
                        SetViewPosition(GetDescriptor(avatar));
                    else
                        status = "Please set the avatar.";
                }

                if (GUILayout.Button("Visemes"))
                {
                    if (avatar)
                        SetVisemes(GetDescriptor(avatar));
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Remove", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("All Scripts"))
                {
                    if (avatar)
                    {
                        DestroyDescriptor(avatar);
                        DestroyBlueprint(avatar);
                    }
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Avatar Descriptor"))
                {
                    if (avatar)
                        DestroyDescriptor(avatar);
                    else
                        status = "Please set the avatar.";
                }

                if (GUILayout.Button("Pipeline Manager"))
                {
                    if (avatar)
                        DestroyBlueprint(avatar);
                    else
                        status = "Please set the avatar.";
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5f);
            statsState = EditorGUILayout.Foldout(statsState, "Avatar Stats", true, EditorStyles.foldout);
            if (statsState)
            {
                EditorGUILayout.LabelField("GameObjects: ", objectCount);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Skinned Mesh Renderers: ", skinCount);
                EditorGUILayout.LabelField("Meshes: ", meshCount);
                EditorGUILayout.LabelField("Triangles: ", triangles);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Materials: ", materials);
                EditorGUILayout.LabelField("Shaders: ", shaders);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("RigidBodies: ", rigidbodies);
                EditorGUILayout.LabelField("Colliders: ", colliders);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Dynamic Bones: ", dynamicBones);
                EditorGUILayout.LabelField("Dynamic Bone Colliders: ", dynamicColliders);
                EditorGUILayout.LabelField("Dynamic Bone Affected: ", dynamicColliderAffectedTransforms);
                GUILayout.Space(10f);
                EditorGUILayout.LabelField("Particle Systems: ", particleSystems);
                EditorGUILayout.LabelField("Max Particles: ", maxParticles);
            }

            GUILayout.Space(10f);
            EditorGUILayout.LabelField("Status: ", status);
            EditorGUILayout.EndScrollView();
        }

        public void AddComponents()
        {
            var pipelineManager = avatar.GetOrAddComponent<PipelineManager>();

            var descriptor = avatar.GetOrAddComponent<VRC_AvatarDescriptor>();

            if (descriptor)
                PopulateVRChat(descriptor);
            else
                status = "Please install VRCSDK.";

            var animator = avatar.GetOrAddComponent<Animator>();

            if (animator)
                PopulateDynamicBones(animator);
            else
                status = "Please ensure your avatar's animator has the avatar set properly.";

        }

        public void PopulateVRChat(VRC_AvatarDescriptor descriptor)
        {
            SetViewPosition(descriptor);
            SetCustomAnimations(descriptor);
            SetVisemes(descriptor);
        }

        public void SetViewPosition(VRC_AvatarDescriptor descriptor)
        {
            descriptor.ViewPosition.y = GetAnimator(avatar).GetBoneTransform(HumanBodyBones.LeftEye).transform.position.y;
            descriptor.ViewPosition.z = 0.05f;
        }

        public void SetCustomAnimations(VRC_AvatarDescriptor descriptor)
        {
            if (sitting)
                descriptor.CustomSittingAnims = sitting;
            else
                status = "Set the sitting override under custom animators.";

            if(standing)
                descriptor.CustomStandingAnims = standing;
            else
                status = "Set the sitting override under custom animators.";

            descriptor.Animations = animationSet;
            descriptor.ScaleIPD = true;
        }

        public void SetVisemes(VRC_AvatarDescriptor descriptor)
        {
            descriptor.lipSync = VRC_AvatarDescriptor.LipSyncStyle.VisemeBlendShape;
            descriptor.VisemeSkinnedMesh = avatar.GetComponentInChildren<SkinnedMeshRenderer>();

            string[] visemes = {"vrc.v_sil", "vrc.v_pp", "vrc.v_ff", "vrc.v_th", "vrc.v_dd", "vrc.v_kk",
                "vrc.v_ch", "vrc.v_ss", "vrc.v_nn", "vrc.v_rr", "vrc.v_aa", "vrc.v_e", "vrc.v_ih",
                "vrc.v_oh", "vrc.v_ou"};

            descriptor.VisemeBlendShapes = visemes;
        }

        public void PopulateDynamicBones(Animator animator)
        {
            SetHeadDynamicBones(animator);
            SetChestDynamicBones(animator);
            SetHipDynamicBones(animator);
        }

        public void SetHeadDynamicBones(Animator animator)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head).gameObject;
            var dynamicBone = head.GetComponent<DynamicBone>() ?? head.AddComponent<DynamicBone>();

            dynamicBone.m_Root = head.transform;
            dynamicBone.m_UpdateRate = 90;
            dynamicBone.m_UpdateMode = DynamicBone.UpdateMode.Normal;
            dynamicBone.m_Damping = 0.2f;
            dynamicBone.m_Elasticity = 0.04f;
            dynamicBone.m_Stiffness = 0.2f;
            dynamicBone.m_Inert = 0.92f;
            dynamicBone.m_Radius = 0.01f;
            dynamicBone.m_EndLength = 0.5f;

            foreach (Transform bone in FindBonesWithText(head.transform, "eye"))
                dynamicBone.m_Exclusions.Add(bone);
                
            foreach (Transform bone in FindBonesWithText(head.transform, "tongue"))
                dynamicBone.m_Exclusions.Add(bone);
            
        }

        public void SetChestDynamicBones(Animator animator)
        {
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest).gameObject;
            var dynamicBone = chest.GetComponent<DynamicBone>() ?? chest.AddComponent<DynamicBone>();

            dynamicBone.m_Root = chest.transform;
            dynamicBone.m_UpdateRate = 90;
            dynamicBone.m_UpdateMode = DynamicBone.UpdateMode.Normal;
            dynamicBone.m_Damping = 0.2f;
            dynamicBone.m_Elasticity = 0.1f;
            dynamicBone.m_Stiffness = 0.2f;
            dynamicBone.m_Inert = 0.9f;
            dynamicBone.m_Radius = 0.04f;
            dynamicBone.m_EndLength = 0f;
            dynamicBone.m_RadiusDistrib = new AnimationCurve(new Keyframe[] {new Keyframe(0f, 1f), new Keyframe(.867f, 1f), new Keyframe(1f, 0.5f)});

            foreach (Transform bone in FindBonesWithText(chest.transform, "shoulder"))
                dynamicBone.m_Exclusions.Add(bone);
            
            foreach (Transform bone in FindBonesWithText(chest.transform, "neck"))
                dynamicBone.m_Exclusions.Add(bone);
        }

        public void SetHipDynamicBones(Animator animator)
        {
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips).gameObject;
            var dynamicBone = hips.GetComponent<DynamicBone>() ?? hips.AddComponent<DynamicBone>();

            dynamicBone.m_Root = hips.transform;
            dynamicBone.m_UpdateRate = 90;
            dynamicBone.m_UpdateMode = DynamicBone.UpdateMode.Normal;
            dynamicBone.m_Damping = 0.05f;
            dynamicBone.m_Elasticity = 0.075f;
            dynamicBone.m_Stiffness = 0.4f;
            dynamicBone.m_Inert = 0.3f;
            dynamicBone.m_Radius = 0.05f;
            dynamicBone.m_EndLength = 0f;
            dynamicBone.m_EndOffset = new Vector3(0f, -0.01f, -0.02f);

            foreach (Transform bone in FindBonesWithText(hips.transform, "spine"))
                dynamicBone.m_Exclusions.Add(bone);
            
            foreach (Transform bone in FindBonesWithText(hips.transform, "leg"))
                dynamicBone.m_Exclusions.Add(bone);
        }

        private List<Transform> FindBonesWithText(Transform root, string name)
        {
            List<Transform> transforms = new List<Transform>();

            foreach(Transform child in root.GetComponentsInChildren<Transform>())
            {
                if (child.name.ToLower().Contains(name) && child.parent.name == root.name)
                    transforms.Add(child);
            }

            return transforms;
        }

        public void MainToChildren()
        {
            foreach(DynamicBone bone in avatar.GetComponents<DynamicBone>())
            {
                var copyBone = bone.m_Root.gameObject.AddComponent<DynamicBone>();
                TransformDynamicBoneValues(bone, copyBone);
                DestroyImmediate(bone);
            }
            status = "Bones succesfully transferred.";
        }

        public void ChildrenToMain()
        {
            foreach (DynamicBone bone in avatar.GetComponents<DynamicBone>())
            {
                var copyBone = avatar.AddComponent<DynamicBone>();
                TransformDynamicBoneValues(bone, copyBone);
                DestroyImmediate(bone);
            }
            status = "Bones succesfully transferred.";
        }

        public void FuckOffDynamicChildren()
        {
            foreach(DynamicBone bone in avatar.GetComponentsInChildren<DynamicBone>())
            {
                if(bone.gameObject != avatar)
                    DestroyImmediate(bone);
            }
            status = "Children bones no longer exist!";
        }

        public void FuckOffMainBones()
        {
            foreach (DynamicBone bone in avatar.GetComponentsInChildren<DynamicBone>())
            {
                if (bone.gameObject == avatar)
                    DestroyImmediate(bone);
            }
            status = "Main bones no longer exist!";
        }

        public void RemoveAllBones()
        {
            foreach (DynamicBone bone in avatar.GetComponentsInChildren<DynamicBone>())
                DestroyImmediate(bone);
            
            status = "Bones no longer exist!";
        }

        public static void CopyMissingObjectsOver(GameObject from, GameObject to)
        {
            List<GameObject> transferObjects = new List<GameObject>();

            Transform[] toTransforms = to.GetComponentsInChildren<Transform>();
            Transform[] fromTransforms = from.GetComponentsInChildren<Transform>();

            var objectCount = 0;

            for (int i = 0; i < fromTransforms.Length; i++)
            {
                if (fromTransforms[i].name != toTransforms[i - objectCount].name)
                {
                    if (i == 0)
                        continue;

                    objectCount++;

                    var parent = fromTransforms[i].parent;
                    var newObject = GameObject.Instantiate<GameObject>(fromTransforms[i].gameObject);

                    foreach(Transform transform in toTransforms)
                    {
                        if(transform.name == parent.name)
                        {
                            newObject.transform.parent = transform;
                            newObject.transform.position = fromTransforms[i].position;
                            newObject.transform.rotation = fromTransforms[i].rotation;
                            newObject.transform.localScale = fromTransforms[i].localScale;
                            newObject.name = fromTransforms[i].name;
                            break;
                        }
                    }
                }
            }
        }

        public static void MoveMissingObjectsOver(GameObject from, GameObject to)
        {
            List<GameObject> transferObjects = new List<GameObject>();

            Transform[] toTransforms = to.GetComponentsInChildren<Transform>();
            Transform[] fromTransforms = from.GetComponentsInChildren<Transform>();

            var objectCount = 0;

            for (int i = 0; i < fromTransforms.Length; i++)
            {
                if (fromTransforms[i].name != toTransforms[i - objectCount].name)
                {
                    if (i == 0)
                        continue;

                    objectCount++;

                    var parent = fromTransforms[i].parent;
                    
                    foreach (Transform transform in toTransforms)
                    {
                        if (transform.name == parent.name)
                        {
                            fromTransforms[i].transform.parent = transform;
                            break;
                        }
                    }
                }
            }
        }

        public static void TransformDynamicBoneValues(DynamicBone from, DynamicBone to)
        {
            to.m_Colliders = from.m_Colliders;
            to.m_Damping = from.m_Damping;
            to.m_DampingDistrib = from.m_DampingDistrib;
            to.m_DistanceToObject = from.m_DistanceToObject;
            to.m_DistantDisable = from.m_DistantDisable;
            to.m_Elasticity = from.m_Elasticity;
            to.m_ElasticityDistrib = from.m_ElasticityDistrib;
            to.m_EndLength = from.m_EndLength;
            to.m_EndOffset = from.m_EndOffset;
            to.m_Exclusions = from.m_Exclusions;
            to.m_Force = from.m_Force;
            to.m_FreezeAxis = from.m_FreezeAxis;
            to.m_Gravity = from.m_Gravity;
            to.m_Inert = from.m_Inert;
            to.m_InertDistrib = from.m_InertDistrib;
            to.m_Radius = from.m_Radius;
            to.m_RadiusDistrib = from.m_RadiusDistrib;
            to.m_ReferenceObject = from.m_ReferenceObject;
            to.m_Root = from.m_Root;
            to.m_Stiffness = from.m_Stiffness;
            to.m_StiffnessDistrib = from.m_StiffnessDistrib;
            to.m_UpdateMode = from.m_UpdateMode;
            to.m_UpdateRate = from.m_UpdateRate;
        }

        public static Animator GetAnimator(GameObject avatar)
        {
            return avatar.GetOrAddComponent<Animator>();
        }

        public static VRC_AvatarDescriptor GetDescriptor(GameObject avatar)
        {
            return avatar.GetOrAddComponent<VRC_AvatarDescriptor>();
        }

        public static void DestroyBlueprint(GameObject avatar)
        {
            DestroyImmediate(avatar.GetComponent<PipelineManager>());
        }

        public static void DestroyDescriptor(GameObject avatar)
        {
            DestroyImmediate(GetDescriptor(avatar));
        }
    }
}
