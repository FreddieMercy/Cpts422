from __future__ import unicode_literals

from django.contrib import admin

import datetime
from .models import API
#from .views import changeAll
#from django import forms
import requests
import json
import collections

from django.db import models

from .models import API
import datetime
from django.utils import timezone, formats
from django.contrib.admin.utils import NestedObjects, quote
import decimal
from django.contrib import admin
from django.contrib.auth import get_permission_codename
from django.shortcuts import render_to_response
from django.db.models.constants import LOOKUP_SEP
from django.db.models.deletion import Collector
from django.forms.forms import pretty_name

from django.utils.html import format_html
from django.utils.text import capfirst
from django.utils import timezone
from django.utils.encoding import force_str, force_text, smart_text
from django.utils import six
from django.utils.translation import ungettext
from django.core.urlresolvers import reverse, NoReverseMatch

from django.contrib import messages
from django.contrib.admin import helpers
from django.contrib.admin.utils import model_ngettext
from django.core.exceptions import PermissionDenied
from django.db import router
from django.template.response import TemplateResponse

from django.utils.translation import ugettext as _, ugettext_lazy



def get_changed_objects(objs, opts, user, admin_site, using):

    collector = NestedObjects(using=using)
    collector.collect(objs)
    perms_needed = set()

    def format_callback(obj):
        has_admin = obj.__class__ in admin_site._registry
        opts = obj._meta

        no_edit_link = '%s: %s' % (capfirst(opts.verbose_name), force_text(obj))

        if has_admin:
            try:
                admin_url = reverse('%s:%s_%s_change'%(admin_site.name, opts.app_label, opts.model_name), None, (quote(obj._get_pk_val()),))
            except NoReverseMatch:

                return no_edit_link

            p = '%s.%s' % (opts.app_label, get_permission_codename('delete', opts))
            if not user.has_perm(p):
                perms_needed.add(opts.verbose_name)

            return format_html('{0}: <a href="{1}">{2}</a>', capfirst(opts.verbose_name), admin_url, obj)
        else:

            return no_edit_link

    to_change = collector.nested(format_callback)

    protected = [format_callback(obj) for obj in collector.protected]

    return to_change, collector.model_count, perms_needed, protected


def changeAll(modeladmin, request, queryset):

		    if not modeladmin.has_delete_permission(request):
			raise PermissionDenied

		    using = router.db_for_write(modeladmin.model)

		    changeable_objects, model_count, perms_needed, protected = get_changed_objects(queryset, modeladmin.model._meta, request.user, modeladmin.admin_site, using)

		    if request.POST.get('post'):

			    if  perms_needed:
				raise PermissionDenied
			    else:
				n = queryset.count()
				if n:
				            for _api in queryset:
						_api.api_oauth=request.POST.get(_api.api_text)
						if _api.api_text == "Spotify":
							url = "https://api.spotify.com/v1/users/"+_api.api_usr +"/playlists/"+_api.api_playlist
							headers = {'content-type': 'application/json', 'Authorization':'Bearer %s'%_api.api_oauth}
							something = json.loads(requests.get(url, headers=headers).text)
							_jsons = {}
							for _obj in something['tracks']['items']:
								song = _obj['track']['uri'][14:36]
								img = _obj['track']['album']['images'][0]['url']
								for i in _obj['track']['artists']:
									try:
										_jsons[i['name']]

									except KeyError:
										_jsons[i['name']] = [None, [None]]
										_jsons[i['name']][0] = img
										_jsons[i['name']][1][0] = song
									else:
										_jsons[i['name']][1].append(song)
							od = collections.OrderedDict(sorted(_jsons.items()))
							_api_keys=[]
							for keys in od:
								_api_keys.append(keys)						
							_api.api_json = json.dumps(od)
							_api.api_keys = json.dumps(_api_keys)
						super(API, _api).save()
					    queryset.update(edit_date = datetime.datetime.today())
					    	
				modeladmin.message_user(request, _("Successfully changed %(count)d %(items)s.") % {
						"count": n, "items": model_ngettext(modeladmin.opts, n)
					    }, messages.SUCCESS)
		    	    return None



		    if len(queryset) == 1:
			objects_name = force_text(modeladmin.model._meta.verbose_name)
		    else:
			objects_name = force_text(modeladmin.model._meta.verbose_name_plural)

		    if  perms_needed or protected:
			title = _("Cannot change %(name)s") % {"name": objects_name}
		    else:
			title = _("Are you sure?")

		    context = dict(
			modeladmin.admin_site.each_context(request),
			title=title,
			objects_name=objects_name,
			changeable_objects=[changeable_objects],
       			model_count=dict(model_count).items(),
			queryset=queryset,
			perms_lacking= perms_needed,
			protected=protected,
			opts=modeladmin.model._meta,
        		action_checkbox_name=helpers.ACTION_CHECKBOX_NAME,
		    )




		    #return TemplateResponse(request, modeladmin.change_api_json_template or ["admin/change_api_json.html"], context, current_app=modeladmin.admin_site.name)
		    return TemplateResponse(request, "admin/change_api_json.html", context, current_app=modeladmin.admin_site.name)
		    #return render_to_response("admin/change_api_json.html", context)



changeAll.short_description = " *** Change All Selected OAuth *** "


class apiAdmin(admin.ModelAdmin):

	fieldsets = [('Date',  {'fields':['edit_date']}  ),('API info', {'fields':['api_text','api_oauth','api_usr','api_playlist','api_json'] }) ]
	list_display = ('api_text', 'edit_date', 'was_edit_recently')
	search_fields = ['api_text']	
	actions=[changeAll]
	list_filter = ['edit_date']




	

admin.site.register(API, apiAdmin)
