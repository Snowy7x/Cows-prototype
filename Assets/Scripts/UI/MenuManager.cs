using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
        [SerializeField] List<Menu> menus;
        [SerializeField] string defaultMenu;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            foreach (var menu in menus)
            {
                menu.menu.SetActive(false);
            }
            
            OpenMenu(defaultMenu);
        }
        
        public void OpenMenu(string name)
        {
            foreach (var menu in menus)
                menu.menu.SetActive(menu.name == name);
        }

        public void OpenMenu(Menu menu)
        {
            OpenMenu(menu.name);
        }
        
        public void CloseMenu(string name)
        {
            foreach (var menu in menus)
            {
                if (menu.name == name)
                {
                    menu.menu.SetActive(false);
                }
            }
        }
        
        public void CloseMenu(Menu menu)
        {
            CloseMenu(menu.name);
        }
    }
}