# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0011_auto_20150429_1127'),
    ]

    operations = [
        migrations.AddField(
            model_name='api',
            name='api_json',
            field=models.CharField(default='some json', max_length=10000),
            preserve_default=False,
        ),
    ]
