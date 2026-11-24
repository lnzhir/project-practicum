using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindManager : MonoBehaviour
{
	private InputActionRebindingExtensions.RebindingOperation rebindOperation;

	public static RebindManager Instance { set; get; }

	void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Запускает процесс переназначения клавиши для конкретного действия
	/// </summary>
	/// <param name="actionToRebind">Действие, которое меняем (например, m_Player.Roll)</param>
	/// <param name="bindingIndex">Индекс биндинга (0 - первый биндинг, для композитов WASD: 1=Up, 2=Down, 3=Left, 4=Right)</param>
	public void StartRebinding(InputAction actionToRebind, int bindingIndex)
	{
		// 1. Перед ребиндом ОБЯЗАТЕЛЬНО выключаем карту ввода, иначе будут баги
		actionToRebind.actionMap.Disable();

		// Отобразите в UI текст: "Нажмите любую клавишу..."
		Debug.Log($"Ожидание нажатия для {actionToRebind.name}...");

		// 2. Запускаем операцию интерактивного ребинда
		rebindOperation = actionToRebind.PerformInteractiveRebinding(bindingIndex)
			// Исключаем мышь (опционально, чтобы случайно не назначить движение мыши на кнопку)
			.WithControlsExcluding("Mouse") 
			// Отмена ребинда, если игрок нажал Escape
			.WithCancelingThrough("<Keyboard>/escape") 
			// Время ожидания, чтобы избежать дребезга клавиш
			.OnMatchWaitForAnother(0.1f) 
			// Прописываем колбэк при успешном завершении
			.OnComplete(operation => FinishRebind(actionToRebind))
			// Колбэк при отмене
			.OnCancel(operation => CancelRebind(actionToRebind))
			.Start(); // Запуск прослушивания ввода
	}

	private void FinishRebind(InputAction action)
	{
		// Получаем красивое имя новой клавиши для UI (например: "Space", "E", "Left Button")
		string newBindingName = action.GetBindingDisplayString();
		Debug.Log($"Клавиша успешно изменена! Новый биндинг: {newBindingName}");

		// Включаем карту обратно
		action.actionMap.Enable();

		// Освобождаем память от операции
		rebindOperation.Dispose();
		rebindOperation = null;

		// Сохраняем новые настройки
		//SaveCustomBindings(action.actionMap.asset);
	}

	private void CancelRebind(InputAction action)
	{
		Debug.Log("Ребинд отменен игроком.");
		action.actionMap.Enable();

		rebindOperation.Dispose();
		rebindOperation = null;
	}


	// Вызывать при сохранении (например, при выходе из меню настроек)
	public void SaveCustomBindings(InputActionAsset actionAsset)
	{
		// Генерирует JSON строку, содержащую ВСЕ переопределения биндингов игрока
		string jsonPayload = actionAsset.SaveBindingOverridesAsJson();
		PlayerPrefs.SetString("PlayerControlsOverrides", jsonPayload);
		PlayerPrefs.Save();
	}

	// Вызывать ОДИН РАЗ при старте игры (например, в Awake вашего AirplaneController ДО включения карт)
	public void LoadCustomBindings(InputActionAsset actionAsset)
	{
		if (PlayerPrefs.HasKey("PlayerControlsOverrides"))
		{
			string jsonPayload = PlayerPrefs.GetString("PlayerControlsOverrides");
			// Загружает сохраненные оверрайды обратно в ассет
			InputActionRebindingExtensions.LoadBindingOverridesFromJson(actionAsset, jsonPayload);
		}
	}

}




public static class InputActionExtensions
{
	/// <summary>
	/// Находит индекс конкретной части композита по её имени (например, "Up", "Negative").
	/// </summary>
	public static int GetBindingIndexForPart(this InputAction action, string partName)
	{
		var bindings = action.bindings;
		for (int i = 0; i < bindings.Count; i++)
		{
			// Проверяем, является ли биндинг частью композита и совпадает ли имя (без учета регистра)
			if (bindings[i].isPartOfComposite && 
				bindings[i].name.Equals(partName, System.StringComparison.OrdinalIgnoreCase))
			{
				return i;
			}
		}

		// Если ничего не нашли, возвращаем -1
		return -1;
	}
}
