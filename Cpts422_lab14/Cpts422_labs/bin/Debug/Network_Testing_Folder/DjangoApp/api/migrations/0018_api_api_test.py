# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0017_auto_20150504_1748'),
    ]

    operations = [
        migrations.AddField(
            model_name='api',
            name='api_test',
            field=models.CharField(default='s', max_length=100),
            preserve_default=False,
        ),
    ]
