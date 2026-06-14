#!/bin/bash
# 高考数据源可用性检测脚本
# 用于提前验证数据获取渠道是否可用

echo "=== 高考数据源可用性检测 ==="

# 1. 检测省市教育考试院官网
check_exam_board() {
    local province=$1
    local url=$2
    echo "检测 $province 教育考试院: $url"
    curl -s -o /dev/null -w "%{http_code}" --connect-timeout 5 "$url" || echo "连接失败"
}

# 2. 检测院校招生网站
check_university_admission() {
    local university=$1
    local url=$2
    echo "检测 $university 招生网站: $url"
    curl -s -o /dev/null -w "%{http_code}" --connect-timeout 5 "$url" || echo "连接失败"
}

# 3. 检测掌上高考院校页面
check_gaokao_cn() {
    local school_id=$1
    local url="https://www.gaokao.cn/school/$school_id"
    echo "检测掌上高考院校ID $school_id: $url"
    curl -s -o /dev/null -w "%{http_code}" --connect-timeout 5 "$url" || echo "连接失败"
}

# 示例检测
echo ""
echo "=== 上海市教育考试院 ==="
check_exam_board "上海" "https://www.shmeea.edu.cn"

echo ""
echo "=== 重点院校招生网站 ==="
check_university_admission "上海海洋大学" "https://zs.shou.edu.cn"
check_university_admission "上海中医药大学" "https://zs.shutcm.edu.cn"

echo ""
echo "=== 掌上高考院校页面 ==="
check_gaokao_cn "949"  # 需要确认正确的院校ID

echo ""
echo "=== 检测完成 ==="