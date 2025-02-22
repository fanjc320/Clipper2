import matplotlib.pyplot as plt
import numpy as np

# Matplotlib 绘图线
# https://www.runoob.com/matplotlib/matplotlib-line.html

# ypoints = np.array([6, 2, 13, 10])
# # plt.plot(ypoints, linestyle = 'dotted')
# # plt.plot(ypoints, ls = '-.')
# # plt.plot(ypoints, color = 'r')
# # plt.plot(ypoints, c = '#8FBC8F')
# # plt.plot(ypoints, c = 'SeaGreen')
# plt.plot(ypoints, linewidth = '12.5')
# plt.show()

# y1 = np.array([3, 7, 5, 9])
# y2 = np.array([6, 2, 13, 10])
# plt.plot(y1)
# plt.plot(y2)
# plt.show()



x1 = np.array([0, 1, 2, 3])
y1 = np.array([3, 7, 5, 9])
x2 = np.array([0, 1, 2, 3])
y2 = np.array([6, 2, 13, 10])

plt.plot(x1, y1, x2, y2)
plt.show()