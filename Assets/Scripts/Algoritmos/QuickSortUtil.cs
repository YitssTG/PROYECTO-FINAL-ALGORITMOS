using System.Collections.Generic;

public static class QuickSortUtil
{
    public static void QuickSort(List<int> list, int low, int high)
    {
        if (low < high)
        {
            int pivot = Partition(list, low, high);
            QuickSort(list, low, pivot - 1);
            QuickSort(list, pivot + 1, high);
        }
    }

    private static int Partition(List<int> list, int low, int high)
    {
        int pivot = list[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (list[j] < pivot)
            {
                i++;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        (list[i + 1], list[high]) = (list[high], list[i + 1]);

        return i + 1;
    }
}
