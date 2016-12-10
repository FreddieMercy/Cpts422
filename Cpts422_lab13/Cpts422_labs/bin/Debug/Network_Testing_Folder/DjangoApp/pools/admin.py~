from django.contrib import admin

#from .models import Question,Choice
from .models import API

#class ChoiceInline(admin.StackedInline):
#	model = Choice
#	extra = 3


class apiAdmin(admin.ModelAdmin):
	fieldsets = [(None,  {'fields':['pub_date']}  ),( "API info", {'fields':['api_text','api_json'] }) ]
	#['pub_date', 'question_text']
	#inlines = [ChoiceInline]
	list_display = ('api_text', 'pub_date', 'was_published_recently')
	search_fields = ['api_text']	
	
	list_filter = ['pub_date']


admin.site.register(API, apiAdmin)



#admin.site.register(Choice, ChoiceAdmin)
