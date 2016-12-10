from django.conf.urls import patterns, include, url
from django.contrib import admin
from django.views.generic import TemplateView 
from .views import index

urlpatterns = [
    url(r'^$', index, name='home'),
#    url(r'^pools/', include('pools.urls')),
    url(r'^admin/', include(admin.site.urls)),
    url(r'^api/', include('api.urls', namespace="api")),
]
