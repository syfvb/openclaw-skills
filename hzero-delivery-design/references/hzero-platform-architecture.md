# HZERO 平台架构参考

HZERO 基于 Spring Cloud 微服务架构，以 Choerodon 为前端框架。

## 整体架构

```
┌─────────────────────────────────────────────────────────┐
│                    前端 (React + Choerodon UI Pro)       │
├─────────────────────────────────────────────────────────┤
│                    API 网关 (HZERO Gateway)              │
├──────────┬──────────┬──────────┬──────────┬─────────────┤
│ IAM服务   │ 平台服务  │ 消息服务  │ 文件服务  │ 业务模块...  │
├──────────┴──────────┴──────────┴──────────┴─────────────┤
│                   注册中心 (Eureka)                      │
├─────────────────────────────────────────────────────────┤
│                   配置中心 (Nacos/Config)                │
├─────────────────────────────────────────────────────────┤
│                数据层 (MySQL + Redis)                    │
└─────────────────────────────────────────────────────────┘
```

## 核心微服务组件

| 服务 | 职责 | 数据库 |
|------|------|--------|
| hzero-iam | 权限、用户、角色、租户管理 | hzero_platform |
| hzero-platform | 平台基础功能：配置、值集、LOV | hzero_platform |
| hzero-message | 消息通知（邮件、短信、站内信） | hzero_message |
| hzero-file | 文件存储与管理 | hzero_file |
| hzero-oauth | OAuth2 认证、社交登录 | hzero_platform |

## 业务服务开发规范

### 服务命名
- Maven: `{group}.hzero.{service-name}`
- 服务ID: `hzero-{service-name}`
- 数据库: `hzero_{service_name}`

### 关键配置

```yaml
# bootstrap.yml
spring:
  application:
    name: hzero-order
  cloud:
    service-registry:
      auto-registration:
        enabled: true

# ExtraDataManager - 自动注册路由和权限
@Component
public class OrderExtraDataManager implements ExtraDataManager {
    @Override
    public String getName() {
        return "hzero-order";  // 必须与服务名一致
    }
}
```

### 权限自动注册
- 使用 `@Permission` 注解标记 API 权限
- 服务重启时自动注册到 `iam_permission` 表
- 路由自动注册到 `hadm_service_route`

### 部署结构
- Docker 容器化部署
- Nginx 反向代理前端静态资源
- 每个服务独立 JAR 包

### 前端集成
- 使用 Choerodon UI Pro 组件库
- Token 存储到 localStorage key `UVYC3FE78`
- 路由通过 HZERO 菜单配置界面管理
